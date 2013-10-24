[PortableRest 2.0 Beta](http://github.com/AdvancedREI/PortableRest)
=================

PortableRest is a Portable Class Library for implementing REST API clients in other Portable Class Libraries. It leverages JSON.NET for rapid, customizable serialization, as well as the Microsoft.Bcl.Async library for awaitable execution on any platform. It is designed to be largely drop-in compatible with RestSharp, though you will need to make some changes and recompile.

Works on .NET 4.5, Windows Phone 8, and Windows 8.

Design Goals
----------
+ Be able to write one REST client wrapper that can be used on all current- and last-generation Microsoft platforms (without the techniques we have to use for [XamlEssentials](http://github.com/AdvancedREI/XamlEssentials)).
+ Have more control over serialization of property names (so you're not limited to the "remove all dashes" convention of RestSharp).
+ Be able to have objects be easily serializable to local storage using standard techniques, without jumping through a lot of hoops.

More Info
-----------
[Read more about PortableRest on our blog](http://advancedrei.com/blogs/development/introducing-portablerest-v2-cross-platform-rest-client-for-dotnet-apps)


Quick start
-----------

Install the NuGet package: `Install-Package PortableRest -Pre`, clone the repo, `git clone git://github.com/advancedrei/portablerest.git`, or [download the latest release](https://github.com/advancedrei/portablerest/zipball/master).

If you are planning on redistributing your own PortableRest-based client, such as our http://gaug.es Client, you need to change your Portable Profile to .NET 4.5 and Windows Phone 8. Using .NET 4.0, Silverlight, or Windows Phone 7.X will cause this package to fail to install. If you need support for those platforms, create an issue and we'll investigate.


Bug tracker
-----------

Have a bug? Please create an issue here on GitHub that conforms with [necolas's guidelines](https://github.com/necolas/issue-guidelines).

https://github.com/AdvancedREI/PortableRest/issues



Twitter account
---------------

Keep up to date on announcements and more by following AdvancedREI on Twitter, [@AdvancedREI](http://twitter.com/AdvancedREI).



Blog
----

Read more detailed announcements, discussions, and more on [The AdvancedREI Dev Blog](http://advancedrei.com/blogs/development).


Author
-------

**Robert McLaws**

+ http://twitter.com/robertmclaws
+ http://github.com/advancedrei


Copyright and license
---------------------

Copyright 2013 AdvancedREI, LLC.

The MIT License (MIT)

Copyright (c) 2013 AdvancedREI, LLC. and Robert McLaws

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

- The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

- You may not use this software to sell apps in the [Windows Phone Store](http://www.windowsphone.com/en-US/store/publishers?publisherId=AdvancedREI%252c%2BLLC.&appId=42268b66-a8ed-46ea-9355-1287522a7cf9) or Windows Store that replicate functionality from apps distributed by AdvancedREI.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.