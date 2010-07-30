using QLNet;

namespace TestSuite
{
	public class Flag : IObserver
	{
		private bool _up;

		public Flag()
		{
			_up = false;
		}

		public void raise()
		{
			_up = true;
		}
		
		public void lower()
		{
			_up = false;
		}
		
		public bool isUp()
		{
			return _up;
		}
		
		public void update()
		{
			raise();
		}
	}
}