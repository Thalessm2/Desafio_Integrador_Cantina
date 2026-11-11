using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace ConnectionClass;

public class Connection
{
   
    private readonly string StringConnection;

    public Connection(string JsonFileName = "Appsettings.json")
    {
        var BasePath = AppDomain.CurrentDomain.BaseDirectory;

        var builder =
            new ConfigurationBuilder()
            .SetBasePath(BasePath)
            .AddJsonFile(JsonFileName, optional: false, reloadOnChange: true);

        IConfiguration configuration = builder.Build();

        StringConnection = configuration.GetConnectionString("Defalut") ??
            configuration["ConnectionStrings:Default"]
                    ?? configuration["ConnectionString"];

        if (StringConnection == null)
        {
            throw new InvalidOperationException($"String não encontrada in '{JsonFileName}' ");
        }
    }
    public MySqlConnection CriarConexao()
    {
        return new MySqlConnection(StringConnection);
    }
}

       


