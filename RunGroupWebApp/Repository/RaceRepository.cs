using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Repository
{
    public class RaceRepository : IRaceRepository
    {
        private readonly ApplicationDbContext _context;

        public RaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a race to the database.
        /// </summary>
        /// <param name="race"></param>
        /// <returns></returns>
        public bool Add(Race race)
        {
            _context.Add(race);
            return Save();
        }

        /// <summary>
        /// Deletes a race from the database.
        /// </summary>
        /// <param name="race"></param>
        /// <returns></returns>
        public bool Delete(Race race)
        {
            _context.Remove(race);
            return Save();
        }

        /// <summary>
        /// Returns a list of all races.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Race>> GetAll()
        {
            return await _context.Races.ToListAsync();
        }

        /// <summary>
        /// Returns a list of races by city.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Race>> GetAllRacesByCity(string city)
        {
            return await _context.Races.Where(c => c.Address.City.Contains(city)).ToListAsync();
        }

        /// <summary>
        /// Returns a the race searched for by id passed in.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Race> GetByIdAsync(int id)
        {
            return await _context.Races.Include(i => i.Address).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Race> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Races.Include(i => i.Address).AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        /// <summary>
        /// Saves the race to the database.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        /// <summary>
        /// Updates the race to the database.
        /// </summary>
        /// <param name="race"></param>
        /// <returns></returns>
        public bool Update(Race race)
        {
            _context.Update(race);
            return Save();
        }
    }
}
