<Query Kind="Statements">
  <Connection>
    <ID>8aa0c313-be4f-4480-8252-ba6e72321ef2</ID>
    <Persist>true</Persist>
    <Server>DESKTOP-0G9PV2L</Server>
    <NoPluralization>true</NoPluralization>
    <Database>Timereporter</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <NuGetReference>NodaTime</NuGetReference>
  <Namespace>NodaTime</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

var q = Events.ToList();
var staged = new List<Workdays>();
var unstaged = Workdays.ToList();
Workdays.DeleteAllOnSubmit(unstaged);
SubmitChanges();

foreach(var e in q)
{
	string date;
	var instant = Instant.FromUnixTimeMilliseconds(e.Timestamp);
	var zonedDateTime = instant.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());
	LocalDate localDate = zonedDateTime.LocalDateTime.Date;
	date = localDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
	var matchCollection = Regex.Matches(e.Kind, "[A-Z]+");
	var kind = matchCollection[0].Value;
	string type = matchCollection[1].Value;
	
	var w =
	(
		from wd in Workdays
		where wd.Kind == kind
		where wd.Date == date
		select wd
	).SingleOrDefault();

	if (w == null)
	{
		w = w ?? new Workdays { Date = date, Kind = kind };
		Workdays.InsertOnSubmit(w);
	}

	if (type == "MAX") w.Arrival = e.Timestamp;
	if (type == "MIN") w.Departure = e.Timestamp;
	
	var unspecifiedTime = SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).ToDateTimeUnspecified();
	w.Added = unspecifiedTime;
	w.Changed = unspecifiedTime;
	SubmitChanges();
	date.Dump();
}

