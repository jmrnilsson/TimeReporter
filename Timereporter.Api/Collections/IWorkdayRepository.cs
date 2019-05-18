using System.Collections.Generic;
using NodaTime;
using Optional;
using Timereporter.Api.Controllers;
using Timereporter.Core.Models;

namespace Timereporter.Api.Collections
{
	public interface IWorkdayRepository
	{
		List<IWorkdaySlice> Find(LocalDate fromDate, LocalDate exclusiveToDate);
		Option<WorkdaySlice> Find(int date);
		void Save(IWorkdaySlice slices);
		void Save(List<IWorkdaySlice> slices);
	}
}
