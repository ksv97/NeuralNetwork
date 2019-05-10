using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNeuralNetwork
{
	public class NeuralNetwork
	{

		// TODO
		// http://www.aiportal.ru/articles/neural-networks/accelerate-training-speed.html
		// Как вариант, можно добавить параметр "скорость уменьшения параметра скорости обучения"


		// алгоритм https://habr.com/ru/post/313216/

		// Можно переработать процесс остановки обучения НС. Сейчас - это средняя погрешность по всем выходам не больше эпсилон
		// Можно сделать критерий остановки - каждый выход НС дает погрешность не большую, чем эпсилон

		public readonly double learningSpeed;
		public readonly double alpha;
		public readonly int neuronsCount;
		public readonly int maxEpoch;

		public double Epsilon;
		
		public const int InputsCount = 2;
		public const int OutputsCount = 5;


		public List<Neuron>[] Neurons { set; get; }
		public List<Synaps> Synapses { set; get; }
		
		public double Error { set; get; }

		public double Momentum
		{
			get
			{
				Random r = new Random();
				//return r.NextDouble() / 3.0;
				return 0.1;
			}
		}

		public NeuralNetwork()
		{
		}

		public NeuralNetwork(double learningSpeed, double alpha, int neuronsCount, int epoch)
		{
			this.learningSpeed = learningSpeed;
			this.alpha = alpha;
			this.neuronsCount = neuronsCount;
			this.maxEpoch = epoch;
		}

		public void CreateNetwork()
		{
			Neurons = new List<Neuron>[3];
			Synapses = new List<Synaps>();

			// Инициализация выходного слоя
			Neurons[0] = new List<Neuron>();
			for (int i = 0; i < InputsCount; i++)
			{
				Neurons[0].Add(new Neuron(i));
			}

			// Инициализация второго слоя
			Neurons[1] = new List<Neuron>();
			for (int i = 0; i < neuronsCount; i++)
			{
				Neurons[1].Add(new Neuron(i));
			}

			// Инициализация выходного слоя
			Neurons[2] = new List<Neuron>();
			for (int i = 0; i < OutputsCount; i++)
			{
				Neurons[2].Add(new Neuron(i));
			}


			foreach (Neuron neuron1 in Neurons[0])
			{	
				foreach (Neuron neuron2 in Neurons[1])
				{
					Synaps synaps = new Synaps();
					Synapses.Add(synaps);
					neuron1.OutputSynapses.Add(synaps);
					neuron2.InputSynapses.Add(synaps);
					synaps.FirstNeuron = neuron1;
					synaps.LastNeuron = neuron2;
				}
			}

			foreach (Neuron neuron2 in Neurons[1])
			{				
				foreach (Neuron neuron3 in Neurons[2])
				{
					Synaps synaps = new Synaps();
					Synapses.Add(synaps);
					neuron2.OutputSynapses.Add(synaps);
					neuron3.InputSynapses.Add(synaps);
					synaps.FirstNeuron = neuron2;
					synaps.LastNeuron = neuron3;
				}
			}

		}

		public void Handle (List<double> input)
		{
			for (int i = 0; i < input.Count; i++)
			{
				Neurons[0].ElementAt(i).Output = input.ElementAt(i);
			}

			for (int i = 1; i < 3; i++)
			{
				foreach (Neuron neuron in Neurons[i])
				{
					foreach (Synaps synaps in neuron.InputSynapses)
					{
						neuron.Output += synaps.FirstNeuron.Output * synaps.Weight;
					}
					neuron.Output = ActivationFunction(neuron.Output);
				}
			}
		}


		// неправильно! Перечитать статью https://habr.com/ru/post/313216/
		// надо переструктурировать 
		public void TrainExample (Data data)
		{		
			// Пересчет дельты для выходных нейронов
			for (int i = 0; i < Neurons[2].Count; i++)
			{
				Neuron currentNeuron = Neurons[2][i];

				int idealOutput = data.Outputs[i];				

				currentNeuron.Delta = (idealOutput - currentNeuron.Output) * DerivateForActivationFunction(currentNeuron.Output);
			}

			// распространение ошибки на скрытый слой
			for (int i = 0; i < Neurons[1].Count; i++)
			{
				Neuron currentNeuron = Neurons[1][i];
				currentNeuron.Delta = 0;

				// Подсчет суммы произведения веса исходящего синапса с нейроном, к которому идет синапс
				// затем домножение на функцию активации
				foreach (Synaps outputSynaps in currentNeuron.OutputSynapses)
				{
					currentNeuron.Delta += outputSynaps.Weight * outputSynaps.LastNeuron.Delta;
				}
				currentNeuron.Delta *= DerivateForActivationFunction(currentNeuron.Output);
				
				// после изменения дельты нейрона мы обязаны пересчитать веса всех выходных синапсов 
				foreach(Synaps synaps in currentNeuron.OutputSynapses)
				{
					ChangeDeltaForSynaps(synaps);
				}								
			}

			// распространение ошибки на входной слой
			for (int i = 0; i < Neurons[0].Count; i++)
			{
				Neuron currentNeuron = Neurons[0][i];

				// во входном слое у нейронов нет входных синапсов, поэтому их дельты пересчитывать не надо
				// Однако нужно пересчитать веса исходящих синапсов
				foreach (Synaps synaps in currentNeuron.OutputSynapses)
				{
					ChangeDeltaForSynaps(synaps);
				}	
			}		
		}

		public void ChangeDeltaForSynaps (Synaps synaps)
		{
			double gradient = synaps.LastNeuron.Delta * synaps.FirstNeuron.Output;
			double delta = learningSpeed * gradient + synaps.LastWeightChange * Momentum;
			synaps.LastWeightChange = delta;
			synaps.Weight += delta;
		}

		public double DerivateForActivationFunction (double output)
		{
			return (1 - output) * output;
		}

		public void Train (Data[] trainSet)
		{
			Error = 1;
			int counter = 0;
			//while (Error > Epsilon)
			while (counter < maxEpoch)
			{
				
				//ShuffleTrainSet(trainSet);

				// обучение сети
				foreach (Data data in trainSet)
				{
					TrainExample(data);
				}
								
				
				// подсчет ошибки в результате очередного обучения	
				Error = 0;
				foreach (Data data in trainSet)
				{
					double error = 0; // средняя ошибка
					Handle(data.Inputs);
					for (int i = 0; i < Neurons[2].Count; i++)
					{
						Neuron currentNeuron = Neurons[2][i];
						error += Math.Abs(data.Outputs[i] - currentNeuron.Output);
					}
					error /= data.Outputs.Count;
					Error += error * error;
				}

				Error /= trainSet.Length;
				// Console.WriteLine("Error = {0}", Error);		
				counter++;	
			}

		}
		
		// Шаффл алгоритмом Дуршенфельда
		public void ShuffleTrainSet (Data[] trainSet)
		{
			Random r = new Random();
			for (int i = 0; i < trainSet.Length - 1; i++)
			{
				Data buf;
				int j = r.Next(0, i + 1);
				buf = trainSet[i];
				trainSet[i] = trainSet[j];
				trainSet[j] = buf;			
				
			}
		}

		public double ActivationFunction(double x)
		{
			return 1 / (1 + Math.Exp(-x * alpha));
		}

		

		//public void PrintNetwork ()
		//{
		//	for (int i = 0; i < Layers; i++)
		//	{
		//		Console.WriteLine($"Layer №{i}:");
		//		foreach (Neuron neuron in Neurons[i])
		//		{
					
		//		}
		//	}
		//}
	}
}
