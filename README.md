# CbCart
Порядок запуска под ОС Windows:
1. Собрать решение в Visual Studio 2019;
2. Скачать Redis из https://github.com/dmajkic/redis/downloads, запустить redis-server.exe;
3. Скопировать скрипт db/redis-init-db.bat в рабочую папку Redis и запустить оттуда;
4. Запустить веб-сервис: CbCart.Service.exe;
4. В браузере открыть https://localhost:5001/index.html;
5. Через интерфейс браузера создать корзину, добавить/удалить продукты, добавить веб хук;
6. Запустить CbCart.Daemon.exe, он выполняет две функции - удаляет корзины с истекшим сроком, предварительно отправляя её содержимое по адресу, заданному веб хуком, и строит отчёт по оставшимся.
По идее, должен быть настроен регулярный запуск данного демона раз в сутки, например, через стандартный планировщик Windows;
7. Для удаления всех корзин запустить CbCart.Daemon.exe 0