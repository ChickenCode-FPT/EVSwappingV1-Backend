using Application.Common.IRespositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly EVSwappingV2Context _context;

        public PaymentRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task Add(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetById(int id)
        {
            return await _context.Payments.Include(p => p.SwapTransaction).FirstOrDefaultAsync(p => p.PaymentId == id);
        }

        public async Task<List<Payment>> GetAll()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetFilterWithSwapt()
        {
            return await _context.Payments.Include(x => x.SwapTransaction).ToListAsync();
        }

        // Update
        public async Task Update(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        // Delete
        public async Task Delete(int id)
        {
            var payment = await GetById(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
            else
            {
                throw new Exception("Payment not found");
            }
        }
    }
}
