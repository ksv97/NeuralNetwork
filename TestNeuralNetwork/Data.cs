using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNeuralNetwork
{
	public class Data
	{
		public List<double> Inputs { set; get; }
		public List<int> Outputs { set; get; }

		public Data()
		{
			this.Outputs = new List<int>();
			this.Inputs = new List<double>();			

		}		
	}

}
