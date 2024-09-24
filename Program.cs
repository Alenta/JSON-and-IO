using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace JSON_and_IO;

class Program
{
    static readonly HttpClient client = new HttpClient();
    
    static async Task Main()
    {
        //Set up a new Serializer class
        Serializer serializer = new();
        //Set the path of the testfile, which should be an empty .txt file
        string path = "C:/Users/marti/OneDrive/Skrivebord/Kode Test/JSON and IO/Testfile-IO.txt";
        //Set up an array with test-messages
        String[] testArray = {"Hey! Do you think this will work?", 
        "We will see. It might!", 
        "We will try three lines. That seems good enough."};
        //Use the serializer class to write the testarray to a file at the determined path
        serializer.WriteToFile(testArray, path);
        //We set up a new array and fill it with strings we wrote to "Testfile-IO.txt"
        String[] testArray2 = serializer.ReadFile(path);
        //we set up a new path, this time to our .json testobject
        string jsonPath = "test.JSON";
        //We use the serializer to read the information on this object
        serializer.ReadFromJSON(jsonPath);
        //We use the serializer to create a new JSON object in the rootfolder, then write out test strings to it.
        serializer.WriteToJSON("test2.JSON", testArray2[0], testArray2[1], testArray2[2]);

        // Call asynchronous network methods in a try/catch block to handle exceptions.
        try
        {
            //Send a query to the web API. Here we could add logic to let the user input queries.
            //This API allows to specify animal, and amount of facts.
            //We could Console.readline to get Animal and Count, then pass that as a string to the API
            using HttpResponseMessage response = await client.GetAsync("https://cat-fact.herokuapp.com/facts/random?animal_type=cat&amount=1");
            //Throw exception if not successfull
            response.EnsureSuccessStatusCode();
            //We convert the response to a strign
            string responseBody = await response.Content.ReadAsStringAsync();
            // Above three lines can be replaced with simplified line:
            // string responseBody = await client.GetStringAsync(uri);

            Console.WriteLine("Response from cat-fact.herokuapp.com:");
            serializer.WriteToJSON("test3.JSON",responseBody);
            Console.WriteLine(responseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }

}

interface ISerializer
{
    string[] ReadFile(string path);
    string[] ReadFromJSON(string path);
    string WriteToJSON(string path, string line1, string line2, string line3);
    void WriteToJSON(string path, string text);
    void WriteToFile(string[] textToWrite, string path);

    //Unused and empty
    string[] CreateFile(string name, string path);
    //Unused and empty
    string[] DeleteFile(string path);
}

public class Serializer: ISerializer{
    public string[] ReadFile(string path)
    {
        List<String> test = [];
        String? line;
        try 
        {
            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr = new StreamReader(path);
            //Read the first line of text
            line = sr.ReadLine();
            //Continue to read until you reach end of file
            while (line != null)
            {
                test.Add(line);
                //write the line to console window
                Console.WriteLine("Information read with StreamReader: " +line);
                //Read the next line
                line = sr.ReadLine();
            }
            //close the file
            sr.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        return test.ToArray();
    }

    public string[] ReadFromJSON(string path){
        using (StreamReader r = new StreamReader("test.JSON"))  
        {  
            string json = r.ReadToEnd();  
            InformationHolder? info = JsonSerializer.Deserialize<InformationHolder>(json) ?? throw new Exception();
            Console.WriteLine("Information read from JSON: " + info.line1);
            Console.WriteLine("Information read from JSON: " + info.line2);
            Console.WriteLine("Information read from JSON: " + info.line3);
            string[] strings = {info.line1,info.line2,info.line3};
            return strings.ToArray();
        }
    }

    public void WriteToFile(string[] textToWrite, string path)
    {
        try 
        {
            //Pass the file path and file name to the StreamWriter constructor
            StreamWriter sw = new StreamWriter(path);
            //Foreach item in texttowrite
            foreach (var item in textToWrite)
            {
                //Writeline the item to the textfile
                sw.WriteLine(item);
                Console.WriteLine($"Wrote {item} to {sw}");

            }
            //Close the stream
            sw.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }        
    }
    public string[] CreateFile(string name, string path){
        String[] test = [];
        return test;
    }

    public string WriteToJSON(string path, string Line1, string Line2, string Line3){
        var information = new InformationHolder
        {
        line1 = Line1,
        line2 = Line2,
        line3 = Line3
        };
        string jsonString = JsonSerializer.Serialize(information);
        System.IO.StreamWriter file = new System.IO.StreamWriter(@".\"+path);
        file.WriteLine(jsonString);
        file.Close();
        return jsonString;

    }

    public void WriteToJSON(string path, string text){
        System.IO.StreamWriter file = new System.IO.StreamWriter(@".\"+path);
        file.WriteLine(text);
        file.Close();
    }
    public string[] DeleteFile(string path){
        String[] test = [];
        return test;
    }
    
}



public class InformationHolder{
    public String? line1 {get; set;}
    public String? line2 {get; set;}
    public String? line3 {get; set;}
}
