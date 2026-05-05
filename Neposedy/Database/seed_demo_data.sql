SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Login = 'admin')
    BEGIN
        INSERT INTO dbo.Users (FullName, Login, Password, Role)
        VALUES (N'Главный администратор', 'admin', 'admin123', 'Admin');
    END;

    DECLARE @TeacherSeed TABLE
    (
        Login NVARCHAR(50),
        FullName NVARCHAR(150),
        [Password] NVARCHAR(50),
        Phone NVARCHAR(30),
        Education NVARCHAR(150),
        ExperienceYears INT
    );

    INSERT INTO @TeacherSeed (Login, FullName, [Password], Phone, Education, ExperienceYears)
    VALUES
        ('teacher.smirnova', N'Елена Викторовна Смирнова', 'teacher123', N'+7 (999) 101-11-11', N'Педагогическое образование', 12),
        ('teacher.ivanova', N'Марина Сергеевна Иванова', 'teacher123', N'+7 (999) 102-22-22', N'Дошкольная педагогика', 8),
        ('teacher.kuznetsova', N'Ольга Андреевна Кузнецова', 'teacher123', N'+7 (999) 103-33-33', N'Педагог-психолог', 15);

    INSERT INTO dbo.Users (FullName, Login, Password, Role)
    SELECT s.FullName, s.Login, s.[Password], 'Teacher'
    FROM @TeacherSeed s
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Users u
        WHERE u.Login = s.Login
    );

    INSERT INTO dbo.Teachers (UserId, Phone, Education, ExperienceYears)
    SELECT u.Id, s.Phone, s.Education, s.ExperienceYears
    FROM @TeacherSeed s
    INNER JOIN dbo.Users u ON u.Login = s.Login
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Teachers t
        WHERE t.UserId = u.Id
    );

    DECLARE @ParentSeed TABLE
    (
        Login NVARCHAR(50),
        FullName NVARCHAR(150),
        [Password] NVARCHAR(50),
        Phone NVARCHAR(30),
        Email NVARCHAR(100),
        [Address] NVARCHAR(250)
    );

    INSERT INTO @ParentSeed (Login, FullName, [Password], Phone, Email, [Address])
    VALUES
        ('parent.sokolova', N'Анна Петровна Соколова', 'parent123', N'+7 (999) 201-11-11', 'sokolova@mail.ru', N'г. Москва, ул. Лесная, д. 12, кв. 14'),
        ('parent.sokolov', N'Игорь Сергеевич Соколов', 'parent123', N'+7 (999) 201-22-22', 'sokolov@mail.ru', N'г. Москва, ул. Лесная, д. 12, кв. 14'),
        ('parent.morozova', N'Наталья Дмитриевна Морозова', 'parent123', N'+7 (999) 202-33-33', 'morozova@mail.ru', N'г. Москва, ул. Садовая, д. 8, кв. 21'),
        ('parent.lebedeva', N'Светлана Алексеевна Лебедева', 'parent123', N'+7 (999) 203-44-44', 'lebedeva@mail.ru', N'г. Москва, ул. Парковая, д. 18, кв. 7'),
        ('parent.kiselev', N'Владимир Олегович Киселев', 'parent123', N'+7 (999) 204-55-55', 'kiselev@mail.ru', N'г. Москва, ул. Центральная, д. 4, кв. 56'),
        ('parent.fedorova', N'Татьяна Игоревна Федорова', 'parent123', N'+7 (999) 205-66-66', 'fedorova@mail.ru', N'г. Москва, ул. Молодежная, д. 27, кв. 30');

    INSERT INTO dbo.Users (FullName, Login, Password, Role)
    SELECT s.FullName, s.Login, s.[Password], 'Parent'
    FROM @ParentSeed s
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Users u
        WHERE u.Login = s.Login
    );

    INSERT INTO dbo.Parents (UserId, Phone, Email, Address)
    SELECT u.Id, s.Phone, s.Email, s.Address
    FROM @ParentSeed s
    INNER JOIN dbo.Users u ON u.Login = s.Login
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Parents p
        WHERE p.UserId = u.Id
    );

    DECLARE @GroupSeed TABLE
    (
        [Name] NVARCHAR(100),
        AgeRange NVARCHAR(50),
        TeacherLogin NVARCHAR(50),
        [Description] NVARCHAR(300)
    );

    INSERT INTO @GroupSeed ([Name], AgeRange, TeacherLogin, [Description])
    VALUES
        (N'Солнышко', N'3-4 года', 'teacher.smirnova', N'Младшая группа для адаптации и развития базовых навыков.'),
        (N'Ромашка', N'4-5 лет', 'teacher.ivanova', N'Средняя группа с упором на творческие занятия и развитие речи.'),
        (N'Звездочки', N'5-6 лет', 'teacher.kuznetsova', N'Старшая группа с подготовкой к школе и расширенной программой.');

    INSERT INTO dbo.Groups ([Name], AgeRange, TeacherId, [Description])
    SELECT s.Name, s.AgeRange, t.Id, s.Description
    FROM @GroupSeed s
    INNER JOIN dbo.Users u ON u.Login = s.TeacherLogin
    INNER JOIN dbo.Teachers t ON t.UserId = u.Id
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Groups g
        WHERE g.Name = s.Name
    );

    DECLARE @ChildSeed TABLE
    (
        FullName NVARCHAR(150),
        BirthDate DATE,
        Gender NVARCHAR(10),
        GroupName NVARCHAR(100),
        AdmissionDate DATE,
        MedicalInfo NVARCHAR(500),
        AdditionalNotes NVARCHAR(500)
    );

    INSERT INTO @ChildSeed (FullName, BirthDate, Gender, GroupName, AdmissionDate, MedicalInfo, AdditionalNotes)
    VALUES
        (N'Артем Соколов', '2021-03-15', N'М', N'Солнышко', '2024-09-01', N'Аллергия на цитрусовые.', N'Спокойно адаптируется, любит конструкторы.'),
        (N'София Морозова', '2020-11-04', N'Ж', N'Ромашка', '2024-09-01', N'Медицинских ограничений нет.', N'Активно участвует в творческих занятиях.'),
        (N'Максим Лебедев', '2019-08-21', N'М', N'Звездочки', '2023-09-01', N'Нужен контроль режима питания.', N'Проявляет интерес к подготовке к школе.'),
        (N'Ева Киселева', '2021-01-10', N'Ж', N'Солнышко', '2024-10-01', N'Медицинских ограничений нет.', N'Быстро идет на контакт со сверстниками.'),
        (N'Илья Федоров', '2020-05-19', N'М', N'Ромашка', '2024-09-15', N'Легкая пищевая аллергия.', N'Хорошо воспринимает занятия на развитие речи.'),
        (N'Мария Соколова', '2019-12-02', N'Ж', N'Звездочки', '2023-09-01', N'Медицинских ограничений нет.', N'Любит чтение и спокойные игры.'),
        (N'Даниил Орлов', '2020-07-28', N'М', N'Ромашка', '2024-09-01', N'Медицинских ограничений нет.', N'Нуждается в поддержке концентрации на занятиях.'),
        (N'Полина Белова', '2021-06-07', N'Ж', N'Солнышко', '2024-11-01', N'Медицинских ограничений нет.', N'Быстро включается в подвижные игры.');

    INSERT INTO dbo.Children (FullName, BirthDate, Gender, GroupId, AdmissionDate, MedicalInfo, Notes)
    SELECT s.FullName, s.BirthDate, s.Gender, g.Id, s.AdmissionDate, s.MedicalInfo, s.AdditionalNotes
    FROM @ChildSeed s
    INNER JOIN dbo.Groups g ON g.Name = s.GroupName
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Children c
        WHERE c.FullName = s.FullName
    );

    DECLARE @ChildParentSeed TABLE
    (
        ChildName NVARCHAR(150),
        ParentLogin NVARCHAR(50),
        RelationshipType NVARCHAR(50)
    );

    INSERT INTO @ChildParentSeed (ChildName, ParentLogin, RelationshipType)
    VALUES
        (N'Артем Соколов', 'parent.sokolova', N'Мать'),
        (N'Артем Соколов', 'parent.sokolov', N'Отец'),
        (N'Мария Соколова', 'parent.sokolova', N'Мать'),
        (N'Мария Соколова', 'parent.sokolov', N'Отец'),
        (N'София Морозова', 'parent.morozova', N'Мать'),
        (N'Максим Лебедев', 'parent.lebedeva', N'Мать'),
        (N'Ева Киселева', 'parent.kiselev', N'Отец'),
        (N'Илья Федоров', 'parent.fedorova', N'Мать');

    INSERT INTO dbo.ChildParents (ChildId, ParentId, RelationshipType)
    SELECT c.Id, p.Id, s.RelationshipType
    FROM @ChildParentSeed s
    INNER JOIN dbo.Children c ON c.FullName = s.ChildName
    INNER JOIN dbo.Users u ON u.Login = s.ParentLogin
    INNER JOIN dbo.Parents p ON p.UserId = u.Id
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.ChildParents cp
        WHERE cp.ChildId = c.Id AND cp.ParentId = p.Id
    );

    DECLARE @AttendanceSeed TABLE
    (
        ChildName NVARCHAR(150),
        AttendanceDate DATE,
        [Status] NVARCHAR(30),
        Comment NVARCHAR(300)
    );

    INSERT INTO @AttendanceSeed (ChildName, AttendanceDate, Status, Comment)
    VALUES
        (N'Артем Соколов', '2026-04-21', N'Присутствовал', N''),
        (N'Артем Соколов', '2026-04-22', N'Присутствовал', N''),
        (N'Артем Соколов', '2026-04-23', N'Отсутствовал', N'Отсутствие по семейным обстоятельствам.'),
        (N'Артем Соколов', '2026-04-24', N'Присутствовал', N''),
        (N'Артем Соколов', '2026-04-25', N'Болел', N'По информации родителей ребенок находится дома по болезни.'),
        (N'София Морозова', '2026-04-21', N'Присутствовал', N''),
        (N'София Морозова', '2026-04-22', N'Отсутствовал', N'Отсутствие по семейным обстоятельствам.'),
        (N'София Морозова', '2026-04-23', N'Присутствовал', N''),
        (N'София Морозова', '2026-04-24', N'Болел', N'По информации родителей ребенок находится дома по болезни.'),
        (N'София Морозова', '2026-04-25', N'Присутствовал', N''),
        (N'Максим Лебедев', '2026-04-21', N'Отсутствовал', N'Отсутствие по семейным обстоятельствам.'),
        (N'Максим Лебедев', '2026-04-22', N'Присутствовал', N''),
        (N'Максим Лебедев', '2026-04-23', N'Болел', N'По информации родителей ребенок находится дома по болезни.'),
        (N'Максим Лебедев', '2026-04-24', N'Присутствовал', N''),
        (N'Максим Лебедев', '2026-04-25', N'Присутствовал', N''),
        (N'Ева Киселева', '2026-04-21', N'Присутствовал', N''),
        (N'Ева Киселева', '2026-04-22', N'Болел', N'По информации родителей ребенок находится дома по болезни.'),
        (N'Ева Киселева', '2026-04-23', N'Присутствовал', N''),
        (N'Ева Киселева', '2026-04-24', N'Присутствовал', N''),
        (N'Ева Киселева', '2026-04-25', N'Отсутствовал', N'Отсутствие по семейным обстоятельствам.'),
        (N'Илья Федоров', '2026-04-21', N'Болел', N'По информации родителей ребенок находится дома по болезни.'),
        (N'Илья Федоров', '2026-04-22', N'Присутствовал', N''),
        (N'Илья Федоров', '2026-04-23', N'Присутствовал', N''),
        (N'Илья Федоров', '2026-04-24', N'Отсутствовал', N'Отсутствие по семейным обстоятельствам.'),
        (N'Илья Федоров', '2026-04-25', N'Присутствовал', N''),
        (N'Мария Соколова', '2026-04-21', N'Присутствовал', N''),
        (N'Мария Соколова', '2026-04-22', N'Присутствовал', N''),
        (N'Мария Соколова', '2026-04-23', N'Отсутствовал', N'Отсутствие по семейным обстоятельствам.'),
        (N'Мария Соколова', '2026-04-24', N'Присутствовал', N''),
        (N'Мария Соколова', '2026-04-25', N'Болел', N'По информации родителей ребенок находится дома по болезни.'),
        (N'Даниил Орлов', '2026-04-21', N'Присутствовал', N''),
        (N'Даниил Орлов', '2026-04-22', N'Отсутствовал', N'Отсутствие по семейным обстоятельствам.'),
        (N'Даниил Орлов', '2026-04-23', N'Присутствовал', N''),
        (N'Даниил Орлов', '2026-04-24', N'Болел', N'По информации родителей ребенок находится дома по болезни.'),
        (N'Даниил Орлов', '2026-04-25', N'Присутствовал', N''),
        (N'Полина Белова', '2026-04-21', N'Отсутствовал', N'Отсутствие по семейным обстоятельствам.'),
        (N'Полина Белова', '2026-04-22', N'Присутствовал', N''),
        (N'Полина Белова', '2026-04-23', N'Болел', N'По информации родителей ребенок находится дома по болезни.'),
        (N'Полина Белова', '2026-04-24', N'Присутствовал', N''),
        (N'Полина Белова', '2026-04-25', N'Присутствовал', N'');

    INSERT INTO dbo.Attendance (ChildId, [Date], [Status], Comment)
    SELECT c.Id, s.AttendanceDate, s.Status, s.Comment
    FROM @AttendanceSeed s
    INNER JOIN dbo.Children c ON c.FullName = s.ChildName
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Attendance a
        WHERE a.ChildId = c.Id AND a.[Date] = s.AttendanceDate
    );

    DECLARE @NoteSeed TABLE
    (
        ChildName NVARCHAR(150),
        TeacherLogin NVARCHAR(50),
        Title NVARCHAR(100),
        [Description] NVARCHAR(500),
        NoteDate DATETIME
    );

    INSERT INTO @NoteSeed (ChildName, TeacherLogin, Title, Description, NoteDate)
    VALUES
        (N'Артем Соколов', 'teacher.smirnova', N'Адаптация', N'Хорошо включился в режим дня и активно участвует в играх.', '2026-04-22T10:00:00'),
        (N'Ева Киселева', 'teacher.smirnova', N'Коммуникация', N'Проявляет инициативу в совместных играх и быстро находит контакт с детьми.', '2026-04-24T11:00:00'),
        (N'Полина Белова', 'teacher.smirnova', N'Подвижные игры', N'С интересом участвует в музыкально-ритмических занятиях.', '2026-04-26T09:00:00'),
        (N'София Морозова', 'teacher.ivanova', N'Творчество', N'Отлично справилась с заданием по рисованию и проявила аккуратность.', '2026-04-25T12:00:00'),
        (N'Илья Федоров', 'teacher.ivanova', N'Речь', N'Стал увереннее отвечать на вопросы во время группового занятия.', '2026-04-27T13:00:00'),
        (N'Даниил Орлов', 'teacher.ivanova', N'Концентрация', N'Лучше удерживает внимание, если задание разбито на короткие этапы.', '2026-04-28T10:00:00'),
        (N'Максим Лебедев', 'teacher.kuznetsova', N'Подготовка к школе', N'Уверенно выполняет задания на счет и распознавание букв.', '2026-04-23T14:00:00'),
        (N'Мария Соколова', 'teacher.kuznetsova', N'Чтение', N'Проявляет интерес к чтению и внимательно слушает рассказы.', '2026-04-29T15:00:00');

    INSERT INTO dbo.Notes (ChildId, TeacherId, NoteDate, Title, Description)
    SELECT c.Id, t.Id, s.NoteDate, s.Title, s.Description
    FROM @NoteSeed s
    INNER JOIN dbo.Children c ON c.FullName = s.ChildName
    INNER JOIN dbo.Users u ON u.Login = s.TeacherLogin
    INNER JOIN dbo.Teachers t ON t.UserId = u.Id
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Notes n
        WHERE n.ChildId = c.Id
          AND n.TeacherId = t.Id
          AND n.NoteDate = s.NoteDate
          AND ISNULL(n.Title, N'') = ISNULL(s.Title, N'')
    );

    DECLARE @AnnouncementSeed TABLE
    (
        GroupName NVARCHAR(100),
        TeacherLogin NVARCHAR(50),
        Title NVARCHAR(150),
        [Description] NVARCHAR(1000),
        PublishDate DATETIME
    );

    INSERT INTO @AnnouncementSeed (GroupName, TeacherLogin, Title, Description, PublishDate)
    VALUES
        (N'Солнышко', 'teacher.smirnova', N'Сменная одежда', N'Просьба принести комплект сменной одежды по сезону.', '2026-04-20T09:00:00'),
        (N'Солнышко', 'teacher.smirnova', N'Прогулка', N'Завтра планируется длительная прогулка, подготовьте удобную обувь.', '2026-05-02T08:00:00'),
        (N'Ромашка', 'teacher.ivanova', N'Творческая неделя', N'На неделе пройдет цикл творческих занятий, можно принести материалы для поделок.', '2026-04-21T10:00:00'),
        (N'Ромашка', 'teacher.ivanova', N'Фотоотчет', N'В пятницу будет размещен фотоотчет о занятиях группы.', '2026-05-03T16:00:00'),
        (N'Звездочки', 'teacher.kuznetsova', N'Подготовка к школе', N'Продолжаем выполнять упражнения на развитие внимания и логики.', '2026-04-22T11:00:00'),
        (N'Звездочки', 'teacher.kuznetsova', N'Собрание', N'В понедельник состоится родительское собрание по вопросам подготовки к школе.', '2026-05-01T18:00:00');

    INSERT INTO dbo.Announcements (GroupId, TeacherId, Title, Description, PublishDate)
    SELECT g.Id, t.Id, s.Title, s.Description, s.PublishDate
    FROM @AnnouncementSeed s
    INNER JOIN dbo.Groups g ON g.Name = s.GroupName
    INNER JOIN dbo.Users u ON u.Login = s.TeacherLogin
    INNER JOIN dbo.Teachers t ON t.UserId = u.Id
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Announcements a
        WHERE a.GroupId = g.Id
          AND a.TeacherId = t.Id
          AND a.PublishDate = s.PublishDate
          AND a.Title = s.Title
    );

    DECLARE @ApplicationSeed TABLE
    (
        ParentFullName NVARCHAR(150),
        Phone NVARCHAR(30),
        Email NVARCHAR(100),
        ChildFullName NVARCHAR(150),
        ChildBirthDate DATE,
        DesiredGroup NVARCHAR(100),
        Comment NVARCHAR(500),
        ApplicationDate DATETIME,
        [Status] NVARCHAR(30)
    );

    INSERT INTO @ApplicationSeed (ParentFullName, Phone, Email, ChildFullName, ChildBirthDate, DesiredGroup, Comment, ApplicationDate, Status)
    VALUES
        (N'Екатерина Романова', N'+7 (999) 301-11-11', 'romanova@mail.ru', N'Кирилл Романов', '2022-02-14', N'Солнышко', N'Интересует зачисление с сентября.', '2026-04-20T10:00:00', N'Новая'),
        (N'Алексей Власов', N'+7 (999) 302-22-22', 'vlasov@mail.ru', N'Алиса Власова', '2021-09-05', N'Ромашка', N'Ребенок ранее посещал частный сад.', '2026-04-22T12:00:00', N'В обработке'),
        (N'Мария Котова', N'+7 (999) 303-33-33', 'kotova@mail.ru', N'Денис Котов', '2020-04-09', N'Звездочки', N'Нужна группа с подготовкой к школе.', '2026-04-25T14:00:00', N'Принята'),
        (N'Ольга Николаева', N'+7 (999) 304-44-44', 'nikolaeva@mail.ru', N'Варвара Николаева', '2022-07-01', N'Солнышко', N'Просьба связаться после 18:00.', '2026-04-27T16:00:00', N'Отклонена');

    INSERT INTO dbo.Applications (ParentFullName, Phone, Email, ChildFullName, ChildBirthDate, DesiredGroup, Comment, ApplicationDate, Status)
    SELECT s.ParentFullName, s.Phone, s.Email, s.ChildFullName, s.ChildBirthDate, s.DesiredGroup, s.Comment, s.ApplicationDate, s.Status
    FROM @ApplicationSeed s
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Applications a
        WHERE a.ParentFullName = s.ParentFullName
          AND a.ChildFullName = s.ChildFullName
          AND a.ApplicationDate = s.ApplicationDate
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
