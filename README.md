[PortableRest 1.0 Alpha](http://github.com/AdvancedREI/PortableRest)
=================

PortableRest is a Portable Class Library for implementing REST API clients in other Portable Class Libraries. It leverages JSON.NET for rapid, customizable serialization, as well as the Microsoft.Bcl.Async library for awaitable execution on any platform.

This initial release has limited support for simple JSON requests. More options (including XML and hopefully DataContract support) will be available in later releases. 

Quick start
-----------

Clone the repo, `git clone git://github.com/advancedrei/portablerest.git`, or [download the latest release](https://github.com/advancedrei/portablerest/zipball/master).

If you are planning on redistributing your own PortableRest-based client, such as our http://gaug.es Client, you need to change your Portable Profile to .NET 4.5 and Windows Phone 7.5. Using .NET 4, .NET 4.0.3, or Windows Phone 7.0 will cause this package to fail to install. If you need support for those platforms, create an issue and we'll investigate.


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
