CREATE TABLE [dbo].[Table]
(
	[Название_ курса] TEXT NOT NULL PRIMARY KEY , 
    [Шифр_курса] TEXT NOT NULL , 
    [Факультет] TEXT NOT NULL , 
    [Количество_часов] TEXT NOT NULL , 
    [ФИО_преподователя] TEXT NULL , 
    [ФИО_студента] TEXT NULL 
)
