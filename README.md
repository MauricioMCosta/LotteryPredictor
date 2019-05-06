# LotteryPredictor
Sample lottery predictor using neural network (Encog C# framework)

## Introduction
This is a simple predictor for lottery. It's based on Brazil's MEGASENA game.

Basically it excercizes the temporal series given by Encog.

## Building & Running

1) Clone this repository with your preferred GIT client
```
git clone ...
```

2) Make sure you have VSCODE and .NET Framework installed.

3) Restore the nuget packages
```
dotnet restore
```

4) Build it
```
dotnet build
```

5) go to bin directory and find the LotteryPredictor.exe in its subdirectory (it won't be far...)
```
cd bin\Debug\net461
LotteryPredictor -d ..\..\..\megasena.dat
```

Wait for the "predict line" and it's done... you have your guess.

# DISCLAIMER

[In Portuguese]

Este programa nao tem a intenção de sugerir, predizer ou confirmar veementemente sorteios de numeros de Megasena. Apesar de usar os numeros oficiais para treinamento e certificação da rede neural, isso não implica na fidedignidade da sugestão. E detalhe, NÃO SEJA BURRO! Matematicamente é impossivel prever o próximo número em uma sequência ALEATÓRIA!

Use por conta e risco. Se ganhar, que bom, seja rico, faça bom proveito... felizmente foi uma mera coincidência. Se perder, perdeu. Esse programa cai na categoria de bolinhos da sorte! 

[Back to English]

Althought this work is not copyrighted, make sure you're carrying references of me in your work. If you're trying to make some money, let me know first!
