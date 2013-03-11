(function (window)
{
    "use strict";

    if (window.PortableRest !== undefined)
    {
        return;
    }

    window.PortableRest = {};
    
    window.PortableRest.ContentTypes =
        {
            FormUrlEncoded: "application/x-www-form-urlencoded",
            Json: "application/json",
            Xml: "application/xml"
        };

    window.PortableRest.HttpMethod =
        {
            Delete: "DELETE",
            Get: "GET",
            Head: "HEAD",
            Options: "OPTIONS",
            Post: "POST",
            Put: "PUT",
            Trace: "TRACE"
        };

    window.PortableRest.HttpStatusCode =
        {
            Continue: 100,
            SwitchingProtocols: 101,
            Processing: 102,

            OK: 200,
            Created: 201,
            Accepted: 202,
            NonAuthoritativeInformation: 203,
            NoContent: 204,
            ResetContent: 205,
            PartialContent: 206,
            MultiStatus: 207,
            AlreadyReported: 208,
            ImUsed: 226,
            LowOnStorageSpace: 250,

            MultipleChoices: 300,
            MovedPermanently: 301,
            Found: 302,
            SeeOther: 303,
            NotModified: 304,
            UseProxy: 305,
            SwitchProxy: 306,
            TemporaryRedirect: 307,
            PermanentRedirect: 308,

            BadRequest: 400,
            Unauthorized: 401,
            PaymentRequired: 402,
            Forbidden: 403,
            NotFound: 404,
            MethodNotAllowed: 405,
            NotAcceptable: 406,
            ProxyAuthenticationRequired: 407,
            RequestTimeout: 408,
            Conflict: 409,
            Gone: 410,
            LengthRequired: 411,
            PreconditionFailed: 412,
            RequestEntityTooLarge: 413,
            RequestURITooLong: 414,
            UnsupportedMediaType: 415,
            RequestedRangeNotSatisfiable: 416,
            ExpectationFailed: 417,
            ImATeapot: 418,
            UnprocessableEntity: 422,
            Locked: 423,
            FailedDependency: 424,
            UpgradeRequired: 426,
            PreconditionRequired: 428,
            TooManyRequests: 429,
            RequestHeaderFieldsTooLarge: 431,

            InternalServerError: 500,
            NotImplemented: 501,
            BadGateway: 502,
            ServiceUnavailable: 503,
            GatewayTimeout: 504,
            HTTPVersionNotSupported: 505,
            VariantAlsoNegotiates: 506,
            InsufficientStorage: 507,
            LoopDetected: 508,
            NotExtended: 510,
            NetworkAuthenticationRequired: 511
        };

    window.PortableRest.RestRequest = function (resource, method)
    {
        /// <summary>Creates a new RestRequest instance for a given Resource and Method.</summary>
        /// <param name="resource" type="String" optional="true">The specific resource to access.</param>
        /// <param name="method" type="window.PortableRest.HttpMethod" optional="true">The HTTP method to use for the request.</param>
        /// <field name="contentType" type="window.PortableRest.ContentTypes" />
        /// <field name="method" type="window.PortableRest.HttpMethod" />
        /// <field name="resource" type="String" />
        /// <returns type="window.PortableRest.RestRequest" />
        
        if (!(this instanceof window.PortableRest.RestRequest))
        {
            return new window.PortableRest.RestRequest(resource, method, ignoreRoot);
        }

        this._urlSegments = [];
        this._parameters = [];
        this._credentials = null;

        this.contentType = window.PortableRest.ContentTypes.FormUrlEncoded;
        this.method = method || window.PortableRest.HttpMethod.Get;
        this.resource = resource || "";
    };

    window.PortableRest.RestRequest.prototype.addUrlSegment = function (key, value)
    {
        /// <summary></summary>
        /// <param name="key" type="String"></param>
        /// <param name="value" type="String"></param>
        
        this._urlSegments.push({ key: key, value: value });
    };

    window.PortableRest.RestRequest.prototype.addParameter = function (key, value)
    {
        /// <summary></summary>
        /// <param name="key" type="String"></param>
        /// <param name="value" type="Object"></param>
        
        this._parameters.push({ key: key, value: value });
    };

    window.PortableRest.RestRequest.prototype.setCredentials = function (user, password)
    {
        /// <summary></summary>
        /// <param name="user" type="String"></param>
        /// <param name="password" type="String"></param>

        this._credentials = { user: user, password: password };
    };

    window.PortableRest.RestRequest.prototype.clearCredentials = function ()
    {
        this._credentials = null;
    };

    window.PortableRest.RestRequest.prototype._getFormattedResource = function (baseUrl)
    {
        /// <summary></summary>
        /// <param name="baseUrl" type="String"></param>
        /// <returns type="String" />


        for (var urlSegmentsIndex = 0, urlSegmentsLength = this._urlSegments.length; urlSegmentsIndex < urlSegmentsLength; urlSegmentsIndex++)
        {
            var segment = this._urlSegments[urlSegmentsIndex];
            this.resource = this.resource.replace("{" + segment.key + "}", encodeURIComponent(segment.value));
        }

        if ((baseUrl !== null) && (baseUrl !== undefined))
        {
            if ((this.resource !== null) && (this.resource !== undefined) && (this.resource !== "") && (this.resource.indexOf("/") === 0))
            {
                this.resource = this.resource.substr(1);
            }

            this.resource = ((this.resource === null) || (this.resource === undefined) || (this.resource === "")) ? baseUrl : baseUrl + "/" + this.resource;
        }

        return this.resource;
    };

    window.PortableRest.RestRequest.prototype._getRequestBody = function ()
    {
        /// <summary></summary>
        /// <returns type="String" />
        
        var parameters = "";

        switch (this.contentType)
        {
            case window.PortableRest.ContentTypes.FormUrlEncoded:
                for (var parameterIndex = 0, parametersLength = this._parameters.length; parameterIndex < parametersLength; parameterIndex++)
                {
                    var parameter = this._parameters[parameterIndex];
                    parameters = parameters + (parameters.length > 0 ? "&" : "") + encodeURIComponent(parameter.key) + "=" + encodeURIComponent(parameter.value.toString());
                }
                break;
            case window.PortableRest.ContentTypes.Xml:
                throw new Error("Sending XML is not yet supported, but will be added in a future release.");
            case window.PortableRest.ContentTypes.Json:
                parameters = this._parameters.length > 0 ? JSON.stringify(this._parameters[0].value) : "";
                break;
        }

        return parameters;
    };

    window.PortableRest.RestClient = function ()
    {
        /// <summary>Creates a new instance of the RestClient class.</summary>
        /// <field name="baseUrl" type="String" />
        /// <returns type="window.PortableRest.RestClient" />
        
        if (!(this instanceof window.PortableRest.RestClient))
        {
            return new window.PortableRest.RestClient();
        }

        this.baseUrl = "";
        this._headers = [];
        this._defaultCredentials = null;
    };

    window.PortableRest.RestClient.prototype.setDefaultCredentials = function (user, password)
    {
        /// <summary></summary>
        /// <param name="user" type="String"></param>
        /// <param name="password" type="String"></param>

        this._defaultCredentials = { user: user, password: password };
    };

    window.PortableRest.RestClient.prototype.clearDefaultCredentials = function ()
    {
        this._defaultCredentials = null;
    };

    window.PortableRest.RestClient.prototype.addHeader = function (key, value)
    {
        /// <summary>Adds a header for a given string key and string value.</summary>
        /// <param name="key" type="String">The header to add.</param>
        /// <param name="value" type="String">The value of the header being added.</param>
        
        this._headers.push({ key: key, value: value });
    };

    window.PortableRest.RestClient.prototype.execute = function (restRequest, callback)
    {
        /// <summary>Executes an asynchronous request to the given resource and deserializes the response</summary>
        /// <param name="restRequest" type="window.PortableRest.RestRequest">The RestRequest to execute.</param>
        /// <param name="callback" type="Function" optional="true"></param>
        /// <remarks>
        /// The callback function is passed either two or three parameters.
        ///     1. The response. If the content type was XML or JSON and can be parsed then parsed version is here, otherwise just the response as received.
        ///     2. The HTTP Status code. This is the code received from the server and can be compared to PortableRest.HttpStatusCode.
        ///     3. In case the JSON parsing fails this is the error thrown the the parser. Otherwise not passed.
        /// </remarks>

        var url = restRequest._getFormattedResource(this.baseUrl);

        var client = new XMLHttpRequest();

        for (var headerIndex = 0, headersLength = this._headers.length; headerIndex < headersLength; headerIndex++)
        {
            var header = this._headers[headerIndex];
            client.setRequestHeader(header.key, header.value);
        }

        restRequest._credentials = restRequest._credentials || this._defaultCredentials;

        if (restRequest._credentials === null)
        {
            client.open(restRequest.method, url, true);
        }
        else
        {
            client.open(restRequest.method, url, true, restRequest._credentials.user, restRequest._credentials.password);
        }

        var body = null;

        if ((restRequest.method === window.PortableRest.HttpMethod.Post) || (restRequest.method === window.PortableRest.HttpMethod.Put))
        {
            client.setRequestHeader("Content-Type", restRequest.contentType);
            body = restRequest._getRequestBody();
        }

        if ((callback !== undefined) && (callback !== null) && (typeof callback === "function"))
        {
            client.onreadystatechange = function ()
            {
                if (client.readyState === 4)
                {
                    if (client.status === 1223)
                    {
                        client.status = window.PortableRest.HttpStatusCode.NoContent;
                    }
                    var type = client.getResponseHeader("Content-Type");
                    
                    if ((type !== null) && (type.indexOf("xml") !== -1) && (client.responseXML !== null) && (client.responseXML !== undefined))
                    {
                        callback(client.responseXML.firstChild, client.status);
                    }
                    else if ((type !== null) && (type.indexOf("json") !== -1) && (client.responseText !== null))
                    {
                        var responseObject;
                        try
                        {
                            responseObject = JSON.parse(client.responseText);
                        }
                        catch (error)
                        {
                            callback(client.responseText, client.status, error);
                            return;
                        }
                        callback(responseObject, client.status);
                    }
                    else
                    {
                        callback(client.responseText, client.status);
                    }
                }
            };
        }

        client.send(body);
    };

})(window);