using System;
using System.Collections;
using System.Collections.Generic;
using Timereporter.Core.Models;

namespace Timereporter.Core.Collections
{
	public class WorkdayEnum : IEnumerator, IEnumerator<IWorkday>
	{
		private int _position = -1;
		private IWorkday[] _workdays;

		public WorkdayEnum(IWorkday[] workdays)
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

		public IWorkday Current
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

		IWorkday IEnumerator<IWorkday>.Current
		{
			get
			{
				return Current;
			}
		}
	}
}
