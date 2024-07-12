using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Repository
{
    /// <summary>
    /// Club Repository.
    /// </summary>
    public class ClubRepository : IClubRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor to interact with DB.
        /// </summary>
        public ClubRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Adds a Club to the database.
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
        public bool Add(Club club)
        {
            _context.Add(club);
            return Save();
        }

        /// <summary>
        /// Deletes Club.
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
        public bool Delete(Club club)
        {
            _context.Remove(club);
            return Save();
        }

        /// <summary>
        /// Returns list of all clubs.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Club>> GetAll()
        {
            return await _context.Clubs.ToListAsync();
        }

        /// <summary>
        /// Returns specific club searched for by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Club> GetByIdAsync(int id)
        {
            return await _context.Clubs.Include(i => i.Address).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Club> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Clubs.Include(i => i.Address).AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        /// <summary>
        /// Returns list of clubs by city.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Club>> GetClubByCity(string city)
        {
            return await _context.Clubs.Where(c => c.Address.City.Contains(city)).ToListAsync();
        }

        /// <summary>
        /// Saves the club to the database.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        /// <summary>
        /// Updates the club in the database.
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Update(Club club)
        {
            _context.Update(club);
            return Save();
        }
    }
}
