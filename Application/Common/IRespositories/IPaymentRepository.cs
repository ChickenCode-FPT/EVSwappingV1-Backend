using Domain.Models;

namespace Application.Common.IRespositories
{
    public interface IPaymentRepository
    {
        Task Add(Payment payment);
        Task<Payment?> GetById(int id);
        Task<List<Payment>> GetAll();
        Task Update(Payment payment);
        Task Delete(int id);
        Task<IEnumerable<Payment>> GetFilterWithSwapt();
    }
}
