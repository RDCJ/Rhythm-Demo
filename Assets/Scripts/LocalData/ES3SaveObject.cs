public interface IES3SaveObject
{
    public void LoadData();
    public void SaveData();
}

public class ES3SaveObject<T> : IES3SaveObject where T : new()
{
    private string key;
    private string filePath;
    private ES3Settings settings;
    public T Data { get; private set; }

    public ES3SaveObject(string key, string filePath, ES3Settings settings)
    {
        this.key = key;
        this.filePath = filePath;
        this.settings = settings;
        LoadData();
    }

    public void LoadData()
    {
        Data = ES3.Load(key, filePath, new T(), settings);
        if (Data == null)
        {
            Data = new T();
        }
    }

    public void SaveData()
    {
        ES3.Save(key, Data, filePath, settings);
    }
}


