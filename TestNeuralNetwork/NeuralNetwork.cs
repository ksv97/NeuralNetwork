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

		// Можно переработать процесс остановки обучения НС. Сейчас - это средняя погрешность по всем выходам не больше эпсилон
		// Можно сделать критерий остановки - каждый выход НС дает погрешность не большую, чем эпсилон

		public readonly double learningSpeed = 0.7;
		public readonly double alpha = 0.5;
		public readonly int neuronsCount = 18;

		public const double Epsilon = 0.002;
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
				return r.NextDouble() / 3.0;
			}
		}

		public NeuralNetwork()
		{
		}

		public NeuralNetwork(double learningSpeed, double alpha, int neuronsCount)
		{
			this.learningSpeed = learningSpeed;
			this.alpha = alpha;
			this.neuronsCount = neuronsCount;
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

			//Console.Write("Handle result: ");
			//foreach (Neuron neuron in Neurons[2])
			//{
			//	Console.Write($"{neuron.Output} ");
			//}
		}

		public void TrainExample (Data data)
		{		
			// Распространение ошибки на Второй слой
			for (int i = 0; i < Neurons[2].Count; i++)
			{
				Neuron currentNeuron = Neurons[2].ElementAt(i);

				currentNeuron.Delta = data.Outputs.ElementAt(i) - currentNeuron.Output;
				foreach (Synaps synaps in currentNeuron.InputSynapses)
				{
					ChangeDeltaForSynaps(synaps, currentNeuron.Delta);

					// Видимо, вот  здесь нужно добавить тот самый момент при подсчете
					//double delta = synaps.LastWeightChange * Momentum + learningSpeed * synaps.FirstNeuron.Output * currentNeuron.Delta;
					//synaps.LastWeightChange = delta;
					//synaps.Weight += delta;
					//synaps.Weight = synaps.Weight + LearningSpeed * synaps.FirstNeuron.Output * currentNeuron.Delta;
				}
			}

			// распространение ошибки на входной слой
			for (int i = 0; i < Neurons[1].Count; i++)
			{
				Neuron currentNeuron = Neurons[1].ElementAt(i);
				currentNeuron.Delta = 0;

				foreach (Synaps outputSynaps in currentNeuron.OutputSynapses)
				{
					currentNeuron.Delta += outputSynaps.Weight * outputSynaps.LastNeuron.Delta;
				}
				currentNeuron.Delta *= currentNeuron.Output * (1 - currentNeuron.Output);

				foreach (Synaps synaps in currentNeuron.InputSynapses)
				{
					// и вот здесь
					ChangeDeltaForSynaps(synaps, currentNeuron.Delta);

					//double delta = synaps.LastWeightChange * Momentum + learningSpeed * synaps.FirstNeuron.Output * currentNeuron.Delta;
					//synaps.LastWeightChange = delta;
					//synaps.Weight += delta;
					//synaps.Weight += LearningSpeed * synaps.FirstNeuron.Output * currentNeuron.Delta;
				}
			}

			
		}

		public void ChangeDeltaForSynaps (Synaps synaps, double currentNeuronDelta)
		{
			double delta = synaps.LastWeightChange * Momentum + learningSpeed * synaps.FirstNeuron.Output * currentNeuronDelta;
			synaps.LastWeightChange = delta;
			synaps.Weight += delta;
		}

		public void Train (Data[] trainSet)
		{
			//double proc = 25.0;
			//double procMax = 95.0;
			//int countOfSucessResults = 0;
			//int stepNumber = 0;
			//Random r = new Random();

			//while (proc < procMax)
			//{
			//	Data currentTraindData = trainSet[r.Next(0, trainSet.Length)];
			//	Handle(currentTraindData.Inputs);
			//	stepNumber++;
			//	for (int i = 0; i < Neurons[2].Count; i++)
			//	{
			//		Neuron currentNeuron = Neurons[2].ElementAt(i);
			//		if (Math.Round(currentNeuron.Output) == currentTraindData.Outputs.ElementAt(i))
			//		{
			//			countOfSucessResults++;
			//		}
			//	}
			//	proc = countOfSucessResults * 100 / stepNumber  ;
			//	Console.WriteLine($"Proc = {proc}");
			//}

			Error = 1;
			while (Error > Epsilon)
			{
				
					//ShuffleTrainSet(trainSet);
				
									
				Error = 0;
				foreach (Data data in trainSet)
				{
					double error = 0; // средняя ошибка
					Handle(data.Inputs);
					for (int i = 0; i < Neurons[2].Count; i++)
					{
						Neuron currentNeuron = Neurons[2].ElementAt(i);
						error += Math.Abs(data.Outputs.ElementAt(i) - currentNeuron.Output);
					}
					error /= data.Outputs.Count;
					Error += error;

					TrainExample(data);

				}

				Error /= trainSet.Length;
				Console.WriteLine("Error = {0}", Error);			
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
