using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Microsoft.Extensions.CommandLineUtils;

namespace LotteryPredictor
{
    class LottoListResults : List<LottoResult> { }
    class Program
    {
        static bool CreateDatabase(string fileDB, out LottoListResults dbl)
        {
            dbl = new LottoListResults();
            using (var reader = File.OpenText(fileDB))
            {
                var line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(' ', '\t')[2].Split(',');
                    var res = new LottoResult(values.Select(s => double.Parse(s)).ToArray());

                    dbl.Add(res);
                }
            }
            dbl.Reverse();
            return true;
        }

        public static BasicNetwork CreateNetwork(int deep = 20)
        {
            var network = new BasicNetwork();

            network.AddLayer(new BasicLayer(null, true, 6 * deep));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 5 * 6 * deep));
            //network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 5 * 6 * deep));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 5 * 6 * deep));
            network.AddLayer(new BasicLayer(new ActivationLinear(), true, 6));
            network.Structure.FinalizeStructure();

            return network;
        }
        public static double[][] BuildLearningInput(LottoListResults dbl, int deep = 20)
        {
            var learningInput = new double[deep][];
            for (int i = 0; i < deep; ++i)
            {
                learningInput[i] = new double[deep * 6];
                for (int j = 0, k = 0; j < deep; ++j)
                {
                    var idx = 2 * deep - i - j;
                    var data = dbl[idx];
                    learningInput[i][k++] = (double)data.V1;
                    learningInput[i][k++] = (double)data.V2;
                    learningInput[i][k++] = (double)data.V3;
                    learningInput[i][k++] = (double)data.V4;
                    learningInput[i][k++] = (double)data.V5;
                    learningInput[i][k++] = (double)data.V6;
                }
            }
            return learningInput;
        }

        public static double[][] BuildLearningOutput(LottoListResults dbl, int deep = 20)
        {
            var learningOutput = new double[deep][];
            for (int i = 0; i < deep; ++i)
            {
                var idx = deep - 1 - i;
                var data = dbl[idx];
                learningOutput[i] = new double[6]
                {
                            (double)data.V1,
                            (double)data.V2,
                            (double)data.V3,
                            (double)data.V4,
                            (double)data.V5,
                            (double)data.V6
                };
            }
            return learningOutput;
        }
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.Name = "LotteryPredictor";
            app.Description = "ML/Encog C# Lotto (MegaSena) Predictor";
            app.HelpOption("-? | -h | --help");
            var dataOption = app.Option("-d | --data", "training data (tsv file)", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (!dataOption.HasValue())
                {
                    app.ShowHint();
                }
                var fileDB = dataOption.Value();

                try
                {
                    var deep = 20;
                    LottoListResults dbl = null;
                    if (CreateDatabase(fileDB, out dbl))
                    {
                        var network = CreateNetwork(deep);

                        var learningInput = BuildLearningInput(dbl, deep);
                        var learningOutput = BuildLearningOutput(dbl, deep);

                        var trainingSet = new BasicMLDataSet(learningInput, learningOutput);
                        var train = new ResilientPropagation(network, trainingSet);
                        train.NumThreads = Environment.ProcessorCount;
                    START:
                        network.Reset();
                    RETRY:
                        var step = 0;
                        do
                        {
                            train.Iteration();
                            Console.WriteLine("Train Error: {0}", train.Error);
                            ++step;
                        } while (train.Error > 0.001 && step < 20);
                        var passedCount = 0;
                        for (var i = 0; i < deep; ++i)
                        {
                            var should = new LottoResult(learningOutput[i]);
                            var inputn = new BasicMLData(6 * deep);
                            Array.Copy(learningInput[i], inputn.Data, inputn.Data.Length);
                            var comput = new LottoResult(((BasicMLData)network.Compute(inputn)).Data);

                            var passed = should.ToString() == comput.ToString();
                            if (passed)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                ++passedCount;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                            Console.WriteLine("{0} {1} {2} {3}",
                                should.ToString().PadLeft(17, ' '),
                                passed ? "==" : "!=",
                                comput.ToString().PadRight(17, ' '),
                                passed ? "PASS" : "FAIL");
                            Console.ResetColor();
                        }
                        var input = new BasicMLData(6 * deep);
                        for (int i = 0, k = 0; i < deep; ++i)
                        {
                            var idx = deep - 1 - i;
                            var data = dbl[idx];
                            input.Data[k++] = (double)data.V1;
                            input.Data[k++] = (double)data.V2;
                            input.Data[k++] = (double)data.V3;
                            input.Data[k++] = (double)data.V4;
                            input.Data[k++] = (double)data.V5;
                            input.Data[k++] = (double)data.V6;
                        }
                        var perfect = dbl[0];
                        var predict = new LottoResult(
                        ((BasicMLData)network.Compute(input)).Data);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Predict: {0}", predict);
                        Console.ResetColor();
                        if (predict.IsOut())
                            goto START;
                        if ((double)passedCount < (deep * (double)9 / (double)10) ||
                          !predict.IsValid())
                            goto RETRY;

                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }


                return 0;
            });
            app.Execute(args);

        }
    }
}
