using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCampus.Core.Entities;

namespace SmartCampus.App.Interfaces
{
    public interface IExamRepository
    {

        Task<IEnumerable<Exam>> GetAllAsync();
        Task<Exam?> GetByIdAsync(int id);
        Task AddAsync(Exam exam);
        Task UpdateAsync(Exam exam);
        Task DeleteAsync(Exam exam);
        Task SaveChangesAsync();

    }
}
