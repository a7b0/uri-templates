Resta.UriTemplates
==================

.NET implementation of the URI template spec ([RFC6570](http://tools.ietf.org/html/rfc6570)):

* Supports up to level 4 template expressions
* Fluent API for manipulating URI templates
* Strong validation and error reporting
* Precompiled URI templates
* Partial resolve of URI templates
* It is passes all tests defined by the [uritemplate-test](https://github.com/uri-templates/uritemplate-test) suite.
* .NET 2.0 and higher

Install
-------

Install via [NuGet package](https://www.nuget.org/packages/Resta.UriTemplates):

	PM> Install-Package Resta.UriTemplates


Examples
--------

Resolve a URI template:

	var template = new UriTemplate("http://example.org/{area}/news{?type,count}");
    
	var uri = template.Resolve(new Dictionary<string, object>
	{
		{ "area", "world" },
		{ "type", "actual" },
		{ "count", "10" }
	});
    
	Assert.AreEqual("http://example.org/world/news?type=actual&count=10", uri);

Resolve a URI template using fluent interface:

	var template = new UriTemplate("http://example.org/{area}/news{?type}");
    
	var uri = template.GetResolver()
		.Bind("area", "world")
		.Bind("type", new string[] { "it", "music", "art" } )
		.Resolve();
    
	Assert.AreEqual("http://example.org/world/news?type=it,music,art", uri);

Construct a URI template:

    var template = new UriTemplateBuilder()
        .Literal("http://example.org/")
        .Simple(new VarSpec("area"))
        .Literal("/last-news")
        .Query("type", new VarSpec("count"))
        .Build();
	
	Assert.AreEqual("http://example.org/{area}/news{?type,count}", template.ToString());

Partial resolve a URI template:

	var template = new UriTemplate("http://example.org/{area}/news{?type,count}");
	var partiallyResolved = template.GetResolver().Bind("count", "10").ResolveTemplate();
    
	Assert.AreEqual("http://example.org/{area}/news?count=10{&type}", partiallyResolved.ToString());

**NB!** Partial resolve of expressions "default", "reserved" and "fragment" is not possible for multiple variables.

License
-------

Copyright 2013 Pavel Shkarin

[MIT License](http://mit-license.org/)

[![Bitdeli Badge](https://d2weczhvl823v0.cloudfront.net/a7b0/uri-templates/trend.png)](https://bitdeli.com/free "Bitdeli Badge")
