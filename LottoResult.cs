using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryPredictor
{
    public class LottoResult
    {
        private List<int> V;
        public int V1 { get { return V[0]; } private set { V[0] = value; } }
        public int V2 { get { return V[1]; } private set { V[1] = value; } }
        public int V3 { get { return V[2]; } private set { V[2] = value; } }
        public int V4 { get { return V[3]; } private set { V[3] = value; } }
        public int V5 { get { return V[4]; } private set { V[4] = value; } }
        public int V6 { get { return V[5]; } private set { V[5] = value; } }
        private LottoResult(int n)
        {
            V = new List<int>(n);
        }
        public LottoResult(int v1, int v2, int v3, int v4, int v5, int v6) : this(6)
        {
            V.AddRange(new int[] { v1, v2, v3, v4, v5, v6 });
        }
        public LottoResult(double[] values) : this(values.Length){
            V.AddRange(values.Select(v=>(int)Math.Round(v)));
        }
        // this is a cool validator - Based on heuristics
        public bool IsValid()
        {
            return
            // the numbers should met a good distribution and stay in a range of frequency
            V1 >= 1 && V1 <= 14 &&
            V2 >= 3 && V2 <= 32 &&
            V3 >= 10 && V3 <= 44 &&
            V4 >= 17 && V4 <= 52 &&
            V5 >= 30 && V5 <= 57 &&
            V6 >= 37 && V6 <= 60 &&
            // can't be repeated... I'm taking care of overlaps in each range of frequency
            (V.Count(v=>v==V1)==1)&&
            (V.Count(v=>v==V2)==1)&&
            (V.Count(v=>v==V3)==1)&&
            (V.Count(v=>v==V4)==1)&&
            (V.Count(v=>v==V5)==1)&&
            (V.Count(v=>v==V6)==1);
        }
        public bool IsOut()
        {
            return
            !(
           V1 >= 1 && V1 <= 14 &&
            V2 >= 3 && V2 <= 32 &&
            V3 >= 10 && V3 <= 44 &&
            V4 >= 8 && V4 <= 58 &&
            V5 >= 30 && V5 <= 57 &&
            V6 >= 37 && V6 <= 60);
        }
        public override string ToString()
        {
            V.Sort();
            return string.Format(String.Join(",",V));
        }
    }
}
