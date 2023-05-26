using System.IO;

class Program
{ 
    static void Main(string[] args) // ...не пойму как можно передавать URL через аргументы
    {
        Console.Write("Введите путь к папке: ");
        string dirPath = string.Empty;
        dirPath = Console.ReadLine();
        if (Directory.Exists(dirPath)) // проверяем, что директория существует
        {
            long startSize = FolderSize(dirPath); // начальный размер
            Console.WriteLine($"Исходный размер папки: {startSize} байт");
            long[] freeSizeFiles = FolderFilesKills(dirPath, DateTime.Now.AddMinutes(-10)); // получение количество удаленных файлов и папок и зачистка
            long newSize = FolderSize(dirPath); // размер после удаления
            Console.WriteLine($"Освобождено: {startSize - newSize} байт");
            Console.WriteLine($"Удалено файлов: {freeSizeFiles[0]}");
            Console.WriteLine($"Удалено папок: {freeSizeFiles[1]}");
            Console.WriteLine($"Текущий размер папки: {newSize} байт");
        }
        else Console.WriteLine("Папка по указанному пути не найдена...");
    }

    static long[] FolderFilesKills(string path, DateTime dateTime)
    {
        long[] delCount = { 0, 0 }; // [0] - - количество удаленных файлов, [1] - количество удаленных папок
        try
        {
            var files = Directory.GetFiles(path);
            foreach (var f in files)
            {
                if (File.GetLastWriteTime(f) < dateTime) // если файл протух, то удаляем
                {
                    File.Delete(f);
                    delCount[0] += 1; // увеличиваем счетчик удаленных файлов
                }
            }
            var dirs = Directory.GetDirectories(path);
            foreach (var d in dirs)
            {
                if (Directory.GetCreationTime(d) < dateTime) // если папка протухла
                {                    
                    delCount[0] += Directory.GetFiles(d, "*.*", SearchOption.AllDirectories).Length; // прибавляем количество всех вложенных файлов в папку перед удалением
                    Directory.Delete(d, true); // удаляем папку со всем барахлом внутри
                    delCount[1] += 1; // увеличиваем счетчик удаленных папок
                    continue;
                }
                FolderFilesKills(d, dateTime);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return delCount;
    }

    static long FolderSize(string path)
    {
        long size = 0;
        try
        {
            var files = Directory.GetFiles(path);
            FileInfo file = null;
            foreach (var f in files)
            {
                file = new FileInfo(f);
                size = size + file.Length;
            }
            var dirs = Directory.GetDirectories(path);
            foreach (var d in dirs)
            {
                size = size + FolderSize(d);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return size;
    }
}