[PortableRest 1.0 Alpha](http://github.com/AdvancedREI/PortableRest)
=================

PortableRest is a Portable Class Library for implementing REST API clients in other Portable Class Libraries. It leverages JSON.NET for rapid, customizable serialization, as well as the Microsoft.Bcl.Async library for awaitable execution on any platform. It is designed to be largely drop-in compatible with RestSharp, though you will need to make some changes and recompile.

This initial release has limited support for JSON & XML GET requests (JSON through JSON.NET, XML through the DataContractSerializer). PUT, POST, and DELETE support is coming very soon.

Works on .NET 4.0.3, Silverlight 4 & 5, Windows Phone 7.5 & 8, and Windows 8.

Design Goals
----------
+ Be able to write one REST client wrapper that can be used on all current- and last-generation Microsoft platforms (without the techniques we have to use for [XamlEssentials](http://github.com/AdvancedREI/XamlEssentials)).
+ Have more control over serialization of property names (so you're not limited to the "remove all dashes" convention of RestSharp).
+ Be able to have objects be easily serializable to local storage using standard techniques, without jumping through a lot of hoops.


Quick start
-----------

Install the NuGet package: `Install-Package PortableRest -Pre`, clone the repo, `git clone git://github.com/advancedrei/portablerest.git`, or [download the latest release](https://github.com/advancedrei/portablerest/zipball/master).

If you are planning on redistributing your own PortableRest-based client, such as our http://gaug.es Client, you need to change your Portable Profile to .NET 4.0.3 and Windows Phone 7.5. Using .NET 4.0 or Windows Phone 7.0 will cause this package to fail to install. If you need support for those platforms, create an issue and we'll investigate.


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

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this work except in compliance with the License. You may obtain a copy of the License in the LICENSE file, or at:

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and limitations under the License.
