using System.Linq;
using FakeDbSet;
using TournamentReport.Models;

public class TestDbSet<T> : InMemoryDbSet<T> where T : class, IEntity
{
    public override T Find(params object[] keyValues)
    {
        int id = (int)keyValues[0];
        return this.FirstOrDefault(item => item.Id == id);
    }
}
