Зайти в SQL server и подключится к серверу
Нажать на импорт приложения уровня данных и выбрать файл sport.bacpac (она находится в папке sportstore)
После импорта запустить проект
Заменить в каждой форме (.cs формы) строчку: private string connectionString = @"Data Source=localhost;Initial Catalog=sport;Integrated Security=True";
А именно изменить в них "Data Source=localhost", вместо "localhost" пишите название сервера SQL вашего компьютера
После того как везде изменили Data Source запускайте приложение!
