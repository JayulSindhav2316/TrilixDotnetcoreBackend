using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Helpers
{
    public  class GraphHelper
    {
        public static  string GetRandomColor()
        {
            Random randomGen = new Random();
            string randomColorName = Constants.GraphColor[randomGen.Next(Constants.GraphColor.Length)];
            return randomColorName;
        }
       
    }
}
