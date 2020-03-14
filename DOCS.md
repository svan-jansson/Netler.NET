<a name='assembly'></a>
# Netler

## Contents

- [Client](#T-Netler-Client 'Netler.Client')
  - [#ctor(port)](#M-Netler-Client-#ctor-System-Int32- 'Netler.Client.#ctor(System.Int32)')
  - [#ctor(port,hostname)](#M-Netler-Client-#ctor-System-String,System-Int32- 'Netler.Client.#ctor(System.String,System.Int32)')
  - [Dispose()](#M-Netler-Client-Dispose 'Netler.Client.Dispose')
  - [Invoke(route,parameters)](#M-Netler-Client-Invoke-System-String,System-Object[]- 'Netler.Client.Invoke(System.String,System.Object[])')
- [ClientDisconnectBehaviour](#T-Netler-Contracts-ClientDisconnectBehaviour 'Netler.Contracts.ClientDisconnectBehaviour')
  - [DisposeServer](#F-Netler-Contracts-ClientDisconnectBehaviour-DisposeServer 'Netler.Contracts.ClientDisconnectBehaviour.DisposeServer')
  - [KeepAlive](#F-Netler-Contracts-ClientDisconnectBehaviour-KeepAlive 'Netler.Contracts.ClientDisconnectBehaviour.KeepAlive')
  - [ShutdownApplication](#F-Netler-Contracts-ClientDisconnectBehaviour-ShutdownApplication 'Netler.Contracts.ClientDisconnectBehaviour.ShutdownApplication')
- [Code](#T-Netler-Response-Code 'Netler.Response.Code')
  - [Error](#F-Netler-Response-Code-Error 'Netler.Response.Code.Error')
  - [Ok](#F-Netler-Response-Code-Ok 'Netler.Response.Code.Ok')
- [Configuration](#T-Netler-Configuration 'Netler.Configuration')
- [IConfiguration](#T-Netler-Contracts-IConfiguration 'Netler.Contracts.IConfiguration')
  - [GetClientDisconnectBehaviour()](#M-Netler-Contracts-IConfiguration-GetClientDisconnectBehaviour 'Netler.Contracts.IConfiguration.GetClientDisconnectBehaviour')
  - [GetClientPid()](#M-Netler-Contracts-IConfiguration-GetClientPid 'Netler.Contracts.IConfiguration.GetClientPid')
  - [GetPort()](#M-Netler-Contracts-IConfiguration-GetPort 'Netler.Contracts.IConfiguration.GetPort')
  - [GetRoutes()](#M-Netler-Contracts-IConfiguration-GetRoutes 'Netler.Contracts.IConfiguration.GetRoutes')
  - [UseClientDisconnectBehaviour()](#M-Netler-Contracts-IConfiguration-UseClientDisconnectBehaviour-Netler-Contracts-ClientDisconnectBehaviour- 'Netler.Contracts.IConfiguration.UseClientDisconnectBehaviour(Netler.Contracts.ClientDisconnectBehaviour)')
  - [UseClientPid()](#M-Netler-Contracts-IConfiguration-UseClientPid-System-Int32- 'Netler.Contracts.IConfiguration.UseClientPid(System.Int32)')
  - [UsePort()](#M-Netler-Contracts-IConfiguration-UsePort-System-Int32- 'Netler.Contracts.IConfiguration.UsePort(System.Int32)')
  - [UseRoutes(routes)](#M-Netler-Contracts-IConfiguration-UseRoutes-System-Action{Netler-Contracts-IRoutes}- 'Netler.Contracts.IConfiguration.UseRoutes(System.Action{Netler.Contracts.IRoutes})')
- [IRoutes](#T-Netler-Contracts-IRoutes 'Netler.Contracts.IRoutes')
  - [Add(route,method)](#M-Netler-Contracts-IRoutes-Add-System-String,System-Func{System-Object[],System-Object}- 'Netler.Contracts.IRoutes.Add(System.String,System.Func{System.Object[],System.Object})')
  - [Invoke(route,parameters)](#M-Netler-Contracts-IRoutes-Invoke-System-String,System-Object[]- 'Netler.Contracts.IRoutes.Invoke(System.String,System.Object[])')
- [InvalidDataType](#T-Netler-Exceptions-InvalidDataType 'Netler.Exceptions.InvalidDataType')
  - [#ctor(message)](#M-Netler-Exceptions-InvalidDataType-#ctor-System-String- 'Netler.Exceptions.InvalidDataType.#ctor(System.String)')
- [MalformedResponseBytes](#T-Netler-Exceptions-MalformedResponseBytes 'Netler.Exceptions.MalformedResponseBytes')
  - [#ctor(message)](#M-Netler-Exceptions-MalformedResponseBytes-#ctor-System-String- 'Netler.Exceptions.MalformedResponseBytes.#ctor(System.String)')
- [Message](#T-Netler-Message 'Netler.Message')
- [RemoteInvokationFailed](#T-Netler-Exceptions-RemoteInvokationFailed 'Netler.Exceptions.RemoteInvokationFailed')
  - [#ctor(message)](#M-Netler-Exceptions-RemoteInvokationFailed-#ctor-System-String- 'Netler.Exceptions.RemoteInvokationFailed.#ctor(System.String)')
- [Request](#T-Netler-Request 'Netler.Request')
  - [#ctor(route,parameters)](#M-Netler-Request-#ctor-System-String,System-Object[]- 'Netler.Request.#ctor(System.String,System.Object[])')
  - [Parameters](#P-Netler-Request-Parameters 'Netler.Request.Parameters')
  - [Route](#P-Netler-Request-Route 'Netler.Request.Route')
  - [Decode()](#M-Netler-Request-Decode-System-Byte[]- 'Netler.Request.Decode(System.Byte[])')
  - [Encode()](#M-Netler-Request-Encode 'Netler.Request.Encode')
  - [Equals(other)](#M-Netler-Request-Equals-System-Object- 'Netler.Request.Equals(System.Object)')
  - [GetHashCode()](#M-Netler-Request-GetHashCode 'Netler.Request.GetHashCode')
  - [op_Equality()](#M-Netler-Request-op_Equality-Netler-Request,Netler-Request- 'Netler.Request.op_Equality(Netler.Request,Netler.Request)')
  - [op_Inequality()](#M-Netler-Request-op_Inequality-Netler-Request,Netler-Request- 'Netler.Request.op_Inequality(Netler.Request,Netler.Request)')
- [Response](#T-Netler-Response 'Netler.Response')
  - [#ctor(status,data)](#M-Netler-Response-#ctor-Netler-Response-Code,System-Object- 'Netler.Response.#ctor(Netler.Response.Code,System.Object)')
  - [Data](#P-Netler-Response-Data 'Netler.Response.Data')
  - [Status](#P-Netler-Response-Status 'Netler.Response.Status')
  - [Decode()](#M-Netler-Response-Decode-System-Byte[]- 'Netler.Response.Decode(System.Byte[])')
  - [Encode()](#M-Netler-Response-Encode 'Netler.Response.Encode')
  - [Equals(other)](#M-Netler-Response-Equals-System-Object- 'Netler.Response.Equals(System.Object)')
  - [FromNamed\`\`1(status,data)](#M-Netler-Response-FromNamed``1-Netler-Response-Code,``0- 'Netler.Response.FromNamed``1(Netler.Response.Code,``0)')
  - [GetHashCode()](#M-Netler-Response-GetHashCode 'Netler.Response.GetHashCode')
  - [op_Equality()](#M-Netler-Response-op_Equality-Netler-Response,Netler-Response- 'Netler.Response.op_Equality(Netler.Response,Netler.Response)')
  - [op_Inequality()](#M-Netler-Response-op_Inequality-Netler-Response,Netler-Response- 'Netler.Response.op_Inequality(Netler.Response,Netler.Response)')
- [RouteAlreadyDefined](#T-Netler-Exceptions-RouteAlreadyDefined 'Netler.Exceptions.RouteAlreadyDefined')
  - [#ctor(message)](#M-Netler-Exceptions-RouteAlreadyDefined-#ctor-System-String- 'Netler.Exceptions.RouteAlreadyDefined.#ctor(System.String)')
- [RouteMethodCallFailed](#T-Netler-Exceptions-RouteMethodCallFailed 'Netler.Exceptions.RouteMethodCallFailed')
  - [#ctor(message,inner)](#M-Netler-Exceptions-RouteMethodCallFailed-#ctor-System-String,System-Exception- 'Netler.Exceptions.RouteMethodCallFailed.#ctor(System.String,System.Exception)')
- [RouteUndefined](#T-Netler-Exceptions-RouteUndefined 'Netler.Exceptions.RouteUndefined')
  - [#ctor(message)](#M-Netler-Exceptions-RouteUndefined-#ctor-System-String- 'Netler.Exceptions.RouteUndefined.#ctor(System.String)')
- [Routes](#T-Netler-Routes 'Netler.Routes')
  - [Add(route,method)](#M-Netler-Routes-Add-System-String,System-Func{System-Object[],System-Object}- 'Netler.Routes.Add(System.String,System.Func{System.Object[],System.Object})')
  - [Invoke(route,parameters)](#M-Netler-Routes-Invoke-System-String,System-Object[]- 'Netler.Routes.Invoke(System.String,System.Object[])')
- [Server](#T-Netler-Server 'Netler.Server')
  - [Create(configure)](#M-Netler-Server-Create-System-Action{Netler-Contracts-IConfiguration}- 'Netler.Server.Create(System.Action{Netler.Contracts.IConfiguration})')
  - [Start()](#M-Netler-Server-Start 'Netler.Server.Start')
  - [Stop()](#M-Netler-Server-Stop 'Netler.Server.Stop')

<a name='T-Netler-Client'></a>
## Client `type`

##### Namespace

Netler

##### Summary

Client for calling a Netler server over a specific TCP port

<a name='M-Netler-Client-#ctor-System-Int32-'></a>
### #ctor(port) `constructor`

##### Summary

Create a new client to a localhost server listing on a given TCP port

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| port | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | A valid TCP port |

<a name='M-Netler-Client-#ctor-System-String,System-Int32-'></a>
### #ctor(port,hostname) `constructor`

##### Summary

Create a new client to a named server listing on a given TCP port

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| port | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | A valid TCP port |
| hostname | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | A valid hostname |

<a name='M-Netler-Client-Dispose'></a>
### Dispose() `method`

##### Summary



##### Parameters

This method has no parameters.

<a name='M-Netler-Client-Invoke-System-String,System-Object[]-'></a>
### Invoke(route,parameters) `method`

##### Summary

Invokes a method on the Netler server using its route

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| route | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The name of the route |
| parameters | [System.Object[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object[] 'System.Object[]') | The parameters to pass to the method |

<a name='T-Netler-Contracts-ClientDisconnectBehaviour'></a>
## ClientDisconnectBehaviour `type`

##### Namespace

Netler.Contracts

##### Summary

Describes what should happen when the connected client disconnects from the server

<a name='F-Netler-Contracts-ClientDisconnectBehaviour-DisposeServer'></a>
### DisposeServer `constants`

##### Summary

Disposes of the current Netler server instance but keeps the application running

<a name='F-Netler-Contracts-ClientDisconnectBehaviour-KeepAlive'></a>
### KeepAlive `constants`

##### Summary

Keeps the server alive to accept new client connections

<a name='F-Netler-Contracts-ClientDisconnectBehaviour-ShutdownApplication'></a>
### ShutdownApplication `constants`

##### Summary

Shuts down the entire application that's hosting the server (default)

<a name='T-Netler-Response-Code'></a>
## Code `type`

##### Namespace

Netler.Response

##### Summary

The status of a Netler response

<a name='F-Netler-Response-Code-Error'></a>
### Error `constants`

##### Summary

The method invokation failed

<a name='F-Netler-Response-Code-Ok'></a>
### Ok `constants`

##### Summary

The method invokation succeeded

<a name='T-Netler-Configuration'></a>
## Configuration `type`

##### Namespace

Netler

##### Summary

Default configuration parameters for a Netler Server

<a name='T-Netler-Contracts-IConfiguration'></a>
## IConfiguration `type`

##### Namespace

Netler.Contracts

##### Summary

Configuration parameters for starting a Netler Server

<a name='M-Netler-Contracts-IConfiguration-GetClientDisconnectBehaviour'></a>
### GetClientDisconnectBehaviour() `method`

##### Summary

The currently configured client disconnect behaviour

##### Parameters

This method has no parameters.

<a name='M-Netler-Contracts-IConfiguration-GetClientPid'></a>
### GetClientPid() `method`

##### Summary

The currently configured client pid

##### Parameters

This method has no parameters.

<a name='M-Netler-Contracts-IConfiguration-GetPort'></a>
### GetPort() `method`

##### Summary

The currently configured port

##### Parameters

This method has no parameters.

<a name='M-Netler-Contracts-IConfiguration-GetRoutes'></a>
### GetRoutes() `method`

##### Summary

The currently configured routes

##### Parameters

This method has no parameters.

<a name='M-Netler-Contracts-IConfiguration-UseClientDisconnectBehaviour-Netler-Contracts-ClientDisconnectBehaviour-'></a>
### UseClientDisconnectBehaviour() `method`

##### Summary

By passing a client OS pid the Netler Server will automatically react when a client disconnects
(default by shutting down the entire application). Use to change the behaviour.

##### Parameters

This method has no parameters.

<a name='M-Netler-Contracts-IConfiguration-UseClientPid-System-Int32-'></a>
### UseClientPid() `method`

##### Summary

By passing a client OS pid the Netler Server will automatically shut down when the client shuts down

##### Parameters

This method has no parameters.

<a name='M-Netler-Contracts-IConfiguration-UsePort-System-Int32-'></a>
### UsePort() `method`

##### Summary

Which port the Netler Server should listen to

##### Parameters

This method has no parameters.

<a name='M-Netler-Contracts-IConfiguration-UseRoutes-System-Action{Netler-Contracts-IRoutes}-'></a>
### UseRoutes(routes) `method`

##### Summary

Which routes the Netler Server should expose

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| routes | [System.Action{Netler.Contracts.IRoutes}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{Netler.Contracts.IRoutes}') | A mapping of string routes to methods that are executed when the route is called |

<a name='T-Netler-Contracts-IRoutes'></a>
## IRoutes `type`

##### Namespace

Netler.Contracts

##### Summary

Message routing for a Netler Server

<a name='M-Netler-Contracts-IRoutes-Add-System-String,System-Func{System-Object[],System-Object}-'></a>
### Add(route,method) `method`

##### Summary

Adds a new route to the route table

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| route | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The exposed name of the route. Example: "CreateNewFoo" |
| method | [System.Func{System.Object[],System.Object}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Func 'System.Func{System.Object[],System.Object}') | A method to execute for calls to the route |

<a name='M-Netler-Contracts-IRoutes-Invoke-System-String,System-Object[]-'></a>
### Invoke(route,parameters) `method`

##### Summary

Invokes the method that is linked to a route

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| route | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The exposed name of the route. Example: "CreateNewFoo" |
| parameters | [System.Object[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object[] 'System.Object[]') | A list of parameters to pass to the method |

<a name='T-Netler-Exceptions-InvalidDataType'></a>
## InvalidDataType `type`

##### Namespace

Netler.Exceptions

##### Summary

Thrown when trying to encode an object to the Netler binary message format and it fails

<a name='M-Netler-Exceptions-InvalidDataType-#ctor-System-String-'></a>
### #ctor(message) `constructor`

##### Summary

Creates a new instance of the exception with a message describing the details of the error

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | A message describing the error |

<a name='T-Netler-Exceptions-MalformedResponseBytes'></a>
## MalformedResponseBytes `type`

##### Namespace

Netler.Exceptions

##### Summary

Thrown when trying to decode a [Response](#T-Netler-Response 'Netler.Response') from a byte array that is malformed

<a name='M-Netler-Exceptions-MalformedResponseBytes-#ctor-System-String-'></a>
### #ctor(message) `constructor`

##### Summary

Creates a new instance of the exception with a message describing the details of the error

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | A message describing the error |

<a name='T-Netler-Message'></a>
## Message `type`

##### Namespace

Netler

##### Summary

Module for encoding/decoding messages for use with Netler clients and servers

<a name='T-Netler-Exceptions-RemoteInvokationFailed'></a>
## RemoteInvokationFailed `type`

##### Namespace

Netler.Exceptions

##### Summary

Thrown from the [Client](#T-Netler-Client 'Netler.Client') when the remote invokation throws an exception on the server

<a name='M-Netler-Exceptions-RemoteInvokationFailed-#ctor-System-String-'></a>
### #ctor(message) `constructor`

##### Summary

Creates a new instance of the exception with a message describing the details of the error

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | A message describing the error |

<a name='T-Netler-Request'></a>
## Request `type`

##### Namespace

Netler

##### Summary

A request from a Netler Client to a Netler Server

<a name='M-Netler-Request-#ctor-System-String,System-Object[]-'></a>
### #ctor(route,parameters) `constructor`

##### Summary

Creates a new request value object

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| route | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The route for the request |
| parameters | [System.Object[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object[] 'System.Object[]') | The parameters to pass to the method invokation |

<a name='P-Netler-Request-Parameters'></a>
### Parameters `property`

##### Summary

The parameters for the method at the server route

<a name='P-Netler-Request-Route'></a>
### Route `property`

##### Summary

The server route of the request

<a name='M-Netler-Request-Decode-System-Byte[]-'></a>
### Decode() `method`

##### Summary

Decodes a request from the Netler transport format

##### Returns

A byte array containing a Netler request encoded in the Netler transport format

##### Parameters

This method has no parameters.

<a name='M-Netler-Request-Encode'></a>
### Encode() `method`

##### Summary

Encodes the request to the Netler transport format

##### Returns

A byte array that can be parsed by a Netler server

##### Parameters

This method has no parameters.

<a name='M-Netler-Request-Equals-System-Object-'></a>
### Equals(other) `method`

##### Summary



##### Returns

True if the objects are equal

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| other | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | The object to compare with the current object. |

<a name='M-Netler-Request-GetHashCode'></a>
### GetHashCode() `method`

##### Summary



##### Parameters

This method has no parameters.

<a name='M-Netler-Request-op_Equality-Netler-Request,Netler-Request-'></a>
### op_Equality() `method`

##### Summary



##### Parameters

This method has no parameters.

<a name='M-Netler-Request-op_Inequality-Netler-Request,Netler-Request-'></a>
### op_Inequality() `method`

##### Summary

Determines whether the specified object is different from the current object

##### Parameters

This method has no parameters.

<a name='T-Netler-Response'></a>
## Response `type`

##### Namespace

Netler

##### Summary

A response from a Netler Server

<a name='M-Netler-Response-#ctor-Netler-Response-Code,System-Object-'></a>
### #ctor(status,data) `constructor`

##### Summary

Creates a [Response](#T-Netler-Response 'Netler.Response') with a status and an anonymously typed data payload

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| status | [Netler.Response.Code](#T-Netler-Response-Code 'Netler.Response.Code') | The response status code |
| data | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | The anonymous data payload |

<a name='P-Netler-Response-Data'></a>
### Data `property`

##### Summary

The response data payload

<a name='P-Netler-Response-Status'></a>
### Status `property`

##### Summary

The status code of the message ivokation at the Netler server

<a name='M-Netler-Response-Decode-System-Byte[]-'></a>
### Decode() `method`

##### Summary

Decodes a response from the Netler transport format

##### Returns

A byte array containing a Netler Response encoded in the Netler transport format

##### Parameters

This method has no parameters.

<a name='M-Netler-Response-Encode'></a>
### Encode() `method`

##### Summary

Encodes the response to the Netler transport format

##### Returns

A byte array that can be parsed by a Netler server

##### Parameters

This method has no parameters.

<a name='M-Netler-Response-Equals-System-Object-'></a>
### Equals(other) `method`

##### Summary



##### Returns

True if the objects are equal

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| other | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | The object to compare with the current object. |

<a name='M-Netler-Response-FromNamed``1-Netler-Response-Code,``0-'></a>
### FromNamed\`\`1(status,data) `method`

##### Summary

Creates a [Response](#T-Netler-Response 'Netler.Response') with a status and an named data payload.
Using this constructor has a major impact on transport performance, as it uses reflection to convert the type into a serializable anonymous object.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| status | [Netler.Response.Code](#T-Netler-Response-Code 'Netler.Response.Code') | The response status code |
| data | [\`\`0](#T-``0 '``0') | The data payload |

##### Generic Types

| Name | Description |
| ---- | ----------- |
| T | The type of the `data` parameter |

<a name='M-Netler-Response-GetHashCode'></a>
### GetHashCode() `method`

##### Summary



##### Parameters

This method has no parameters.

<a name='M-Netler-Response-op_Equality-Netler-Response,Netler-Response-'></a>
### op_Equality() `method`

##### Summary



##### Parameters

This method has no parameters.

<a name='M-Netler-Response-op_Inequality-Netler-Response,Netler-Response-'></a>
### op_Inequality() `method`

##### Summary

Determines whether the specified object is different from the current object

##### Parameters

This method has no parameters.

<a name='T-Netler-Exceptions-RouteAlreadyDefined'></a>
## RouteAlreadyDefined `type`

##### Namespace

Netler.Exceptions

##### Summary

Thrown when trying to add a [Routes](#T-Netler-Routes 'Netler.Routes') route and it already exists in the route table

<a name='M-Netler-Exceptions-RouteAlreadyDefined-#ctor-System-String-'></a>
### #ctor(message) `constructor`

##### Summary

Creates a new instance of the exception with a message describing the details of the error

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | A message describing the error |

<a name='T-Netler-Exceptions-RouteMethodCallFailed'></a>
## RouteMethodCallFailed `type`

##### Namespace

Netler.Exceptions

##### Summary

Thrown when an invoking a method on a [Routes](#T-Netler-Routes 'Netler.Routes') route and it throws any unhandled exception

<a name='M-Netler-Exceptions-RouteMethodCallFailed-#ctor-System-String,System-Exception-'></a>
### #ctor(message,inner) `constructor`

##### Summary

Creates a new instance of the exception with a message describing the details of the error

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | A message describing the error |
| inner | [System.Exception](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Exception 'System.Exception') | An inner exception containing details of the caught error |

<a name='T-Netler-Exceptions-RouteUndefined'></a>
## RouteUndefined `type`

##### Namespace

Netler.Exceptions

##### Summary

Thrown when trying to invoke a method on a [Routes](#T-Netler-Routes 'Netler.Routes') route and it doesn't exist in the route table

<a name='M-Netler-Exceptions-RouteUndefined-#ctor-System-String-'></a>
### #ctor(message) `constructor`

##### Summary

Creates a new instance of the exception with a message describing the details of the error

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | A message describing the error |

<a name='T-Netler-Routes'></a>
## Routes `type`

##### Namespace

Netler

##### Summary

Message routing for a Netler Server

<a name='M-Netler-Routes-Add-System-String,System-Func{System-Object[],System-Object}-'></a>
### Add(route,method) `method`

##### Summary

Adds a new route to the route table

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| route | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The exposed name of the route. Example: "CreateNewFoo" |
| method | [System.Func{System.Object[],System.Object}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Func 'System.Func{System.Object[],System.Object}') | A method to execute for calls to the route |

<a name='M-Netler-Routes-Invoke-System-String,System-Object[]-'></a>
### Invoke(route,parameters) `method`

##### Summary

Invokes the method that is linked to a route

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| route | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The exposed name of the route. Example: "CreateNewFoo" |
| parameters | [System.Object[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object[] 'System.Object[]') | A list of parameters to pass to the method |

<a name='T-Netler-Server'></a>
## Server `type`

##### Namespace

Netler

##### Summary

A Netler Server listens to incoming TCP requests and translates them into method calls

<a name='M-Netler-Server-Create-System-Action{Netler-Contracts-IConfiguration}-'></a>
### Create(configure) `method`

##### Summary

Creates new Netler Server instance

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| configure | [System.Action{Netler.Contracts.IConfiguration}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{Netler.Contracts.IConfiguration}') | Callback for configuring the server instance |

<a name='M-Netler-Server-Start'></a>
### Start() `method`

##### Summary

Starts a process running the Netler Server

##### Parameters

This method has no parameters.

<a name='M-Netler-Server-Stop'></a>
### Stop() `method`

##### Summary

Stops the Netler Server

##### Parameters

This method has no parameters.
