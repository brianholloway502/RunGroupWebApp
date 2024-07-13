using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;

        public ClubController(IClubRepository clubRepository, IPhotoService photoService)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
        }

        /// <summary>
        /// Returns a list of the clubs.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            IEnumerable<Club> clubs = await _clubRepository.GetAll();
            return View(clubs);
        }

        /// <summary>
        /// Retrieves the club that matches the id of the passed in id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetByIdAsync(id);
            return View(club);
        }

        /// <summary>
        /// Display the Create page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Executes when creating a new Club
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(clubVM.Image);
                var club = new Club
                {
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    Image = result.Url.ToString(),
                    Address = new Address
                    {
                        Street = clubVM.Address.Street,
                        City = clubVM.Address.City,
                        State = clubVM.Address.State,
                    }
                };
                _clubRepository.Add(club);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }

            return View(clubVM);
        }

        /// <summary>
        /// Displays the the current club selected for editing.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null) return View("Error"); // If no club found, give an error.

            var clubVM = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId,
                Address = club.Address,
                URL = club.Image,
                ClubCategory = club.ClubCategory
            };
            return View(clubVM);
        }

        /// <summary>
        /// Post the changes to the server for updating the club.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clubVM"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club");
                return View("Edit", clubVM);
            }

            // Look up the club based on id passed in.
            var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);

            // If club exists, proceed with edits.
            if (userClub != null)
            {
                // Try to delete the old photo.
                try
                {
                    await _photoService.DeletePhotoAsync(userClub.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete the photo");
                    return View(clubVM);
                }

                // Add the new photo the user uploaded for the club.
                var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);

                // Now update the fields for the club.
                var club = new Club
                {
                    Id = id,
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    Image = photoResult.Url.ToString(),
                    AddressId = clubVM.AddressId,
                    Address = clubVM.Address,
                };

                _clubRepository.Update(club);
                return RedirectToAction("Index");
            }
            else
            {
                return View(clubVM);
            }

        }

        /// <summary>
        /// Displays Delete page to the user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int id)
        {
            var clubDetails = await _clubRepository.GetByIdAsync(id);
            if (clubDetails == null)
            {
                return View("Error");
            }

            return View(clubDetails);
        }

        /// <summary>
        /// Handles the pst and the actual deletion of the club from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var clubDetails = await _clubRepository.GetByIdAsync(id);
            if (clubDetails == null)
            {
                return View("Error");
            }

            _clubRepository.Delete(clubDetails);
            return RedirectToAction("Index");
        }
    }
}
