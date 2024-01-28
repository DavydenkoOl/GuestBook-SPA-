using GuestBook_SPA_.Models;
using GuestBook_SPA_.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace GuestBook_SPA_.Controllers
{
    public class MessagesController : Controller
    {
        IRepository<Messages> _repository;
        IRepository<Users> _repo;
        public MessagesController(IRepository<Messages> context, IRepository<Users> repo)
        {
            _repository = context;
            _repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetList());
        }
        [HttpGet]
        public async Task<IActionResult> GetListMessage()
        {
            if (await _repository.GetList() == null)
                return Problem("Посетители на данный момент не оставили ни одного отзыва!");
            var _list_mess = await _repository.GetList();
            string response = JsonConvert.SerializeObject(_list_mess);
            return Json(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetMessagById(int id)
        {
            var mes = await _repository.GetObject(id);
            if (mes == null)
            {
                return NotFound();
            }
            string response = JsonConvert.SerializeObject(mes);
            return Json(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMessag(Messages mess)
        {
            if (mess !=null)
            {
                var mess_up=await _repository.GetObject(mess.Id);
                if (mess_up == null)
                    Problem("Сообщение не найдено!");
                mess_up.Message=mess.Message;
                mess_up.CreatedDate = DateTime.Now; 
                
                 _repository.Update(mess_up);
                await _repository.Save();
                string response = "Отзыв успешно изменён!";
                return Json(response);
            }
            return Problem("Проблемы при редактировании отзыва!");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMessag(int id)
        {
            if (await _repository.GetList() == null)
            {
                return Problem("Посетители на данный момент не оставили ни одного отзыва!");
            }
            var mess_up = await _repository.GetObject(id);
            if (mess_up != null)
            {
               await _repository.Delete(id);
            }
            await _repository.Save();
            string response = "Отзыв успешно удален!";
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> InsertStudent(Messages mess)
        {
            if (mess != null)
            {
                mess.CreatedDate = DateTime.Now;
                mess.Owner = await _repo.GetObject(mess.Id_user);
                await _repository.Create(mess);
                await _repository.Save();
                string response = "Отзыв успешно добавлен!";
                return Json(response);
            }
            return Problem("Проблемы при добавлении отзыва!");
        }
    }
}
