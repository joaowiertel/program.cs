using System;
using System.Data.SQLite;
using System.IO;

class Program
{
    static string dbFile = "Todo.db";
    static string connectionString = $"Data Source={dbFile};Version=3;";

    static void Main()
    {
        InitDatabase();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("== ToDo App ==");
            Console.WriteLine("1. Adicionar tarefa");
            Console.WriteLine("2. Listar tarefas");
            Console.WriteLine("3. Marcar como concluída");
            Console.WriteLine("4. Deletar tarefa");
            Console.WriteLine("5. Sair");
            Console.Write("Escolha: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": AddTask(); break;
                case "2": ListTasks(); break;
                case "3": CompleteTask(); break;
                case "4": DeleteTask(); break;
                case "5": return;
                default: Console.WriteLine("Opção inválida!"); break;
            }

            Console.WriteLine("\nPressione Enter para continuar...");
            Console.ReadLine();
        }
    }

    static void InitDatabase()
    {
        if (!File.Exists(dbFile))
        {
            SQLiteConnection.CreateFile(dbFile);
        }

        using var conn = new SQLiteConnection(connectionString);
        conn.Open();

        string createTable = @"
            CREATE TABLE IF NOT EXISTS Tasks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Description TEXT NOT NULL,
                IsDone INTEGER NOT NULL DEFAULT 0
            );";

        using var cmd = new SQLiteCommand(createTable, conn);
        cmd.ExecuteNonQuery();
    }

    static void AddTask()
    {
        Console.Write("Digite a descrição da tarefa: ");
        var desc = Console.ReadLine();

        using var conn = new SQLiteConnection(connectionString);
        conn.Open();

        string insert = "INSERT INTO Tasks (Description) VALUES (@desc)";
        using var cmd = new SQLiteCommand(insert, conn);
        cmd.Parameters.AddWithValue("@desc", desc);
        cmd.ExecuteNonQuery();

        Console.WriteLine("Tarefa adicionada com sucesso!");
    }

    static void ListTasks()
    {
        using var conn = new SQLiteConnection(connectionString);
        conn.Open();

        string select = "SELECT * FROM Tasks";
        using var cmd = new SQLiteCommand(select, conn);
        using var reader = cmd.ExecuteReader();

        Console.WriteLine("\nLista de tarefas:");
        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var desc = reader.GetString(1);
            var isDone = reader.GetInt32(2) == 1 ? "[X]" : "[ ]";
            Console.WriteLine($"{id}. {isDone} {desc}");
        }
    }

    static void CompleteTask()
    {
        Console.Write("Digite o ID da tarefa a marcar como concluída: ");
        var id = Console.ReadLine();

        using var conn = new SQLiteConnection(connectionString);
        conn.Open();

        string update = "UPDATE Tasks SET IsDone = 1 WHERE Id = @id";
        using var cmd = new SQLiteCommand(update, conn);
        cmd.Parameters.AddWithValue("@id", id);
        var rows = cmd.ExecuteNonQuery();

        Console.WriteLine(rows > 0 ? "Tarefa concluída!" : "Tarefa não encontrada.");
    }

    static void DeleteTask()
    {
        Console.Write("Digite o ID da tarefa a excluir: ");
        var id = Console.ReadLine();

        using var conn = new SQLiteConnection(connectionString);
        conn.Open();

        string delete = "DELETE FROM Tasks WHERE Id = @id";
        using var cmd = new SQLiteCommand(delete, conn);
        cmd.Parameters.AddWithValue("@id", id);
        var rows = cmd.ExecuteNonQuery();

        Console.WriteLine(rows > 0 ? "Tarefa excluída!" : "Tarefa não encontrada.");
    }
}