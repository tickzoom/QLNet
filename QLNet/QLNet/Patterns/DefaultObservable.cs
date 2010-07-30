using System;

namespace QLNet.Patterns
{
	public abstract class DefaultObservable : IObservable
	{
		private event Action notifyObserversEvent;

		public virtual void registerWith(Action handler)
		{
			notifyObserversEvent += handler;
		}

		public virtual void unregisterWith(Action handler)
		{
			notifyObserversEvent -= handler;
		}

		public void notifyObservers()
		{
			Action handler = notifyObserversEvent;
			if (handler != null)
			{
				handler();
			}
		}
	}
}