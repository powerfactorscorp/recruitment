public class MyApplication
{
    public static void Main(string[] args)
    {
        // We want to print out all the candidates, meeting a condition,  
        // and a random job assigned to them into a file. 
        Console.WriteLine("Enter a candidate name:");
        string name = Console.ReadLine();
        List<CandidateJob> jobs = GetCandidatesJobs(name);
        bool done = WriteJobsToTxT(jobs);
    }

    public List<CandidateJob> GetCandidatesJobs(string name)
    {
        List<Candidate> candidates = GetCandidates(name);
        List<CandidateJob> jobsByCandidate;

        foreach (Candidate candidate in candidates)
        {
            jobsByCandidate.Add(new CandidateJob()
            {
                Candidate = candidate,
                Task = GetJobs(candidate.Id)[new Random().Next()]
            });
        }

        return jobsByCandidate;
    }

    public List<Candidate> GetCandidates(string name)
    {
        var result = new List<Candidate>();
        var sqlConnection = new SqlConnection("Server=myServer;Initial Catalog=myDatabase;User ID=myUser;Password=myPassword");
        var sqlCommand = new SqlCommand("SELECT * FROM dbo.Candidate where Name = '" + name + "'", sqlConnection);
        var reader = sqlCommand.ExecuteReader();

        do
        {
            result.Add(new Candidate
            {
                CandidateId = reader.GetInt(0),
                Name = reader.GetString(1),
                Surname = reader.GetString(2)
            });
        }
        while (reader.Read());

        return result;
    }

    public List<Job> GetJobs(int candidateId)
    {
        var result = new List<Job>();
        var sqlConnection = new SqlConnection("Server=myServer;Initial Catalog=myDatabase;User ID=myUser;Password=myPassword");
        var sqlCommand = new SqlCommand("SELECT * FROM dbo.Task where CandidateId = " + candidateId, sqlConnection);
        var reader = sqlCommand.ExecuteReader();

        do
        {
            result.Add(new Job
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                CandidateId = candidateId
            });
        }
        while (reader.Read());

        return result;
    }

    public bool WriteJobsToTxT(List<CandidateJobs> randomJobs)
    {
        StreamWriter sw = new StreamWriter("C:\\randomJobs.txt");
        int countJob = randomJobs.Count;

        for (int index = 0; index <= countJob; ++index)
        {
            CandidateJob candidateJob = randomJobs[index];
            string line = candidateJob.PrintCandidateJob();

            sw.WriteLine(line);
            randomJobs.RemoveAt(index);
        }

        return true;
    }
}

public class Candidate
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}

public class Job
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CandidateId { get; set; }
    public JobDetail Details { get; set; }
}

public class JobDetail
{
    public TimeSpan Duration { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}

public class CandidateJob
{
    public Candidate Candidate { get; set; }
    public Job Job { get; set; }

    public string PrintCandidateTask()
    {
        return Candidate.Id + " " + Candidate.Name + " " + Candidate.Surname + " : " + Task.Name + " in " + Job.Details.ExecutionTime.TotalMinutes;
    }
}
