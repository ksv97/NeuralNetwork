using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNeuralNetwork
{
	public class Neuron
	{
		public double Output { set; get; }
		public List<Synaps> InputSynapses { set; get; }
		public List<Synaps> OutputSynapses { set; get; }
		public double Delta { set; get; }
		public int Id { set; get; }

		public Neuron(int id)
		{
			this.Id = id;
			OutputSynapses = new List<Synaps>();
			InputSynapses = new List<Synaps>();
			Delta = 0;
			Output = 0;
		}
	}
}
