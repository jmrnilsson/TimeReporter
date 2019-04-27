using System;
using System.Collections;
using System.Collections.Generic;
using Timereporter.Core.Models;

namespace Timereporter.Core.Collections
{
	public class WorkdayEnum : IEnumerator, IEnumerator<Workday>
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
			return _position < _workdays.Length;
		}

		public void Reset()
		{
			_position = -1;
		}

		public void Dispose()
		{
			_workdays = null;
			Reset();
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

		Workday IEnumerator<Workday>.Current
		{
			get
			{
				return Current;
			}
		}
	}
}
