# JWT-IdentityApi

<h2>About</h2>
<p>Authentication API using <em>Json Web Token (JWT)</em> and <em>AspNetCore Identity</em>.</p>
<p>Custom Logs using Serilog.</p>
<p>Documented using Swagger UI.</p>

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
