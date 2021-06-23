# JWT-IdentityApi

<h2>About</h2>
<p>Authentication API using <em>Json Web Token (JWT)</em> and <em>AspNetCore Identity</em>.</p>
<p>Custom Logs using Serilog.</p>
<p>Documented using Swagger UI.</p>

<h4>Json Web Token (JWT)</h4>
<p>
    The <a href="https://jwt.io/" target="_blank">JSON Web Token</a>  is an Internet standard for creating data with optional signature and/or 
    encryption whose payload contains JSON that asserts some number of claims. Tokens are signed using a private secret or a public/private key.
</p>

<h4>Microsoft Identity</h4>
<p>
    The <a href="https://docs.microsoft.com/en-us/azure/active-directory/develop/" target="_target">Microsoft Identity</a> platform helps you build applications
    your users and customers can sign in to using their Microsoft identities or social accounts, and provide authorized access to your own APIs or 
    Microsoft APIs like Microsoft Graph.
</p>

<h4>Microsoft SQL Server</h4>
<p>
    <a href="https://www.microsoft.com/en-us/sql-server/sql-server-2019" target="_blank">Microsoft SQL Server</a> is a relational database management 
    system developed by Sybase in partnership with Microsoft. 
    This partnership lasted until 1994, with the release of the version for Windows NT and since then Microsoft maintains the maintenance of the product.
</p>

<h4>Swagger</h4>
<p>
    <a href="https://swagger.io/" target="_blank">Swagger</a> is an Interface Description Language for describing RESTful APIs expressed using JSON. 
    <a href="https://swagger.io/" target="_blank">Swagger</a> is used together with a set of open-source software tools to design, build, document, 
    and use RESTful web services. 
    <a href="https://swagger.io/" target="_blank">Swagger</a> includes automated documentation, code generation, and test-case generation.
</p>

<h2>Tools</h2>
<ul>
    <li>.NET 5.0+</li>
    <li>VS Code</li>
    <li>Microsoft SQL Server</li>
    <li>Swagger UI</li>
</ul>

<h2>How to Run</h2>
<ol>
    <li>Run <code>dotnet restore</code></li>
    <li>Change the ConnectionString in appsettings.json</li>
    <li>Run <code>dotnet ef migrations add {migration name}</code></li>
    <li>Run <code>dotnet ef database update</code></li>
    <li>Run <code>dotnet run</code></li>
</ol>
