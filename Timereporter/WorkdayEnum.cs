using System;
using System.Collections;
using Timereporter.Models;

namespace Timereporter
{
	public class WorkdayEnum : IEnumerator
	{
		private int _position = -1;
		private Workday[] _workdays;

		public WorkdayEnum(Workday[] workdays)
		{
			_workdays = workdays;
		}

		public bool MoveNext()
		{
			_position++;
			return (_position < _workdays.Length);
		}

		public void Reset()
		{
			_position = -1;
		}

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public Workday Current
		{
			get
			{
				try
				{
					return _workdays[_position];
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}
	}
}
