using System;
using System.Collections.Generic;
using System.Text;
using Timereporter.Core.Models;

namespace Timereporter.Core.Collections
{
	public interface ITimes : IDictionary<string, ITime>
	{
	}
}
