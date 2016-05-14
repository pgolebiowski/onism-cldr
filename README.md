[![][build-img]][build]
[![][nuget-img]][nuget]

[build]:     https://ci.appveyor.com/project/PatrykGobiowski/onism-cldr
[build-img]: https://ci.appveyor.com/api/projects/status/hrpmwirp95ib56qb?svg=true
[nuget]:     https://www.nuget.org/packages/Onism.Cldr
[nuget-img]: https://badge.fury.io/nu/Onism.Cldr.svg


# Onism
High-performance CLDR library for .NET. It allows you to build an efficient binary representation of CLDR data and to consume this representation easily.

## Usage
How to get the name of the first month in the Hebrew calendar in British English? Couldn't be easier!

```csharp
var data = CldrData.LoadFromFile("cldr.bin");
var path = "dates.calendars.hebrew.months.format.abbreviated.1";

Console.WriteLine(data.GetValue(path, enGB)); // Tishri
```

## Wiki
The [wiki][0] is the best place to learn about Onism. It includes, but is not limited to:

  * [Getting started][1],
  * [What is CLDR?][2],
  * [Exemplary use case][3],
  * [The meaning of Onism][4].

[0]:https://github.com/pgolebiowski/Onism.Cldr/wiki
[1]:https://github.com/pgolebiowski/Onism.Cldr/wiki/Getting-started
[2]:https://github.com/pgolebiowski/onism-cldr/wiki/About-CLDR
[3]:https://github.com/pgolebiowski/onism-cldr/wiki/Hebrew-month-names
[4]:https://github.com/pgolebiowski/onism-cldr/wiki/The-meaning-of-Onism


## Contributing
[![][gitter-img]][gitter] [![][email-img]](mailto:ortorektyk@gmail.com)

Star the project, give feedback, suggest an improvement you need, or just tell me about your day :smile: Feel free to open an [issue] or to submit a [pull request].

## License
[The MIT License](LICENSE). Basically, you can do whatever you want as long as you include the original copyright and license notice in any copy or substantial use of this work.


[issue]:https://github.com/pgolebiowski/onism-cldr/issues
[pull request]:https://github.com/pgolebiowski/onism-cldr/pulls
[gitter-img]:https://img.shields.io/gitter/room/pgolebiowski/onism-cldr.svg
[gitter]:https://gitter.im/pgolebiowski/onism-cldr?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge
[email-img]:https://img.shields.io/badge/email-to%20ortorektyk%40gmail.com-brightgreen.svg
