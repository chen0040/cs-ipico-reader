# cs-ipico-reader

IPICO reader provides a turnkey sports timing solution for medium to large active sports events using RFID chip technology. This project provides library for reading streaming data and downloading cache files from IPICO elite and IPICO lite reader.

# Usage

Git clone the project, add the project to your C# solution and add the project reference to your main application. Note that the library is written with Target Framework .NET 4.6.

For the usage cases, please refer to the unit tests. Below are some usage scenarios illustrated with sample codes.

### Download log files from ipico reader 

```cs 
List<string> logs = ReaderLog.Files;
foreach (string logname in logs)
{
	Console.WriteLine("Start downloading {0} ...", logname);

	string logpath = Path.Combine("/tmp", logname);
	try
	{
		ReaderUtil.DownloadLog(host, logname, logpath, (ln, perc) =>
		{
			Console.WriteLine("Downloading {0}: {1}%\r", logname, perc);
		});
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.ToString());
	}
}
```

The sample code above downloads 4 different files from the ipico reader (which has a network uri <i>host</i>), the files downloaded are stored in the /tmp folder of the local computer. The downloaded files are from ReaderLog.Files which are listed below:

* infod.log
* ttyS0.log
* ttyS1.log
* FS_LS.log


### Extracting RFID Tag ID and Timing

The source code below shows how to use the library to extract the tag id and timing from the log files from ipico reader

```cs
string logname = ReaderLog.FS_LS;
string logpath = Path.Combine("/tmp", logname);

if (!File.Exists(logpath))
{
	Console.WriteLine("Start downloading {0} ...", logname);

	try
	{
		ReaderUtil.DownloadLog(host, logname, logpath, (ln, perc) =>
		{
			Console.WriteLine("Downloading {0}: {1}%\r", logname, perc);
		});
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.ToString());
	}
}

string line = null;
using (StreamReader reader = new StreamReader(logpath))
{
	while ((line = reader.ReadLine()) != null)
	{
		string tag = ReaderDecoder.ExtractTagID(line);
		DateTime? dt=ReaderDecoder.ExtractDateTime(line);
		if (dt.HasValue)
		{
			Console.WriteLine("{0}:{1}", tag, dt.Value);
		}
	}
}
```


### Read tags and timing with offset start time with ipico reader

The source code below shows how to use the library to read tag ids and timing in real-time from ipico-reader with offset start time

```cs
 string logname = ReaderLog.FS_LS;
DateTime start_time = new DateTime(2013, 10, 29, 15, 14, 0);
List<string> tags = ReaderUtil.ReadTags(host, logname, (fLine) =>
	{
		DateTime? rec_time = ReaderDecoder.ExtractDateTime(fLine);
		if (rec_time != null)
		{
			if (rec_time.Value > start_time)
			{
				return true;
			}
		}
		return false;
	});
foreach (string tag in tags)
{
	Console.WriteLine("Tag: {0}", tag);
}
```

### Other API

The sample code belows show how to obtain the ipico reader clock information

```cs
Console.WriteLine("Reader Time: {0}", ReaderUtil.GetReaderTime(host, ReaderType.Elite));
Console.WriteLine("Time Difference: {0}", ReaderUtil.CompareClocks(host, ReaderType.Elite));

ReaderUtil.UpdateReaderTime(host, ReaderType.Elite);
```

The sample code below show how to check connection with ipico reader

```cs
boolean connected = ReaderUtil.CanConnect2Reader(string host)
```