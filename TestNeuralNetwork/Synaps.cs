using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNeuralNetwork
{
	public class Synaps
	{		
		public double Weight { set; get; }
		//public double PreviousDelta { set; get; }
		public Neuron FirstNeuron { set; get; }
		public Neuron LastNeuron { set; get; }
		public double LastWeightChange { set; get; }

		public Synaps()
		{
			Random r = new Random();
			//PreviousDelta = 0;

			Weight = r.NextDouble() / 100.0;
			LastWeightChange = 0;
		}
	}
}
