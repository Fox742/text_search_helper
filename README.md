# TextSearchHelper

Библиотека (TextSearchHelper), позволяющая осуществлять высокопроизводительный полнотекстовый поиск в файле. Написана мной для тренировки после того, как я столкнулся с подобной задачей в моей рабочей практике, в рамках которой программа должна была осуществлять полнотекстовый поиск в большом текстовом файле. Так процедуру поиска необходимо было осуществлять внушительное количество раз, поэтому стояла задача оптимизации полнотектового поиска.
Для решения данной задачи мной был написан класс TextSearchHelper.TSHelper, получающий путь к файлу, по которому будет осуществлять поиск в дальнейшем. Объект класса TSHelper строит индекс-кеш по содержимому файла, с помощью которого осуществляет высокопроизводительный поиск по файлу при использовании методов find и findAll.

Пример использования:

*using TextSearchHelper;*  
*...*  
*File("some_file.txt").WriteAllText(Оооочень большая строка);*  
*using (TSHelper _helper = new TSHelper("some_file.txt"))*  
*{*  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;*Tuple<long, int>[] InternalResult = _helper.findAll(whatToFind);*    
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;*// Массив InternalResult - содержит результат поиска в виде пар, где Item1 - номер строки, а Item2 - номер символа начала вхождения искомой строки whatToFind*  
*}*  

Построение индекса-кеша может выполняться синхронно в конструкторе или асинхронно в фоновом процессе. Данная возможность управляется путём спейиального флага конструктора класса TSHelper. При асинхронном пострении инлекса-кеша в функциях find/findAll можно задать необходимо ли ждать завершение поятроения кеша или выбросить исключение WaitCacheException.


Класс TSHelper осуществлает наблюдение над файлом и также индексирует новые данные в индекс-кеш с помощью механизма FileSystemWatcher. 
При переименовании или удалении наблюдаемого TSHelper-ом файла TSHelper переводится в состояние "Disposed" и должен быть пересоздан для дальнейшей работой с файлом
В случае попытки вызова методов find/findAll TSHelper в состоянии "Disposed" будет выброшено исключение TextSearchDisposed

На своём компьютере под управлением операционной системы Windows 10 я добился ускорения процедуры полнотекстового поиска более чем в 3 раза
