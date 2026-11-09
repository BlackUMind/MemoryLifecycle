# КТ №41. Параллельная и фоновая сборка мусора
<<<<<<< HEAD
=======


**Workstation**
без нагрузки
https://kili.ru/screen/r/SEDqtR4yJrG33NpTmR65vUb4  

только браузер
https://kili.ru/screen/r/thXO8xF1N00xQjYwBBOI6nTd   

браузер+видео 4к
https://kili.ru/screen/r/9LNNKHA2ZP0OOZFWIt6mG96x  

**Server GC**

без нагрузки
https://kili.ru/screen/r/VaZA3AcSPeTTnyfM3f_WVner  

только браузер
https://kili.ru/screen/r/qv1RzPJcOpiuU_CNMeYmgkCU  

браузер+видео 4к
https://kili.ru/screen/r/MgoA2JuSm1zSODVGV91Eoblc  


**Вывод:**

На практике режим Workstation GC показал лучшее время выполнения во всех сценариях (от 30,60 мс до 48,83 мс против 47,31–105,47 мс у Server GC). Это связано с тем, что при небольшом объеме аллокаций и коротком тесте накладные расходы Server GC на создание и координацию нескольких куч (по одной на каждое логическое ядро процессора) превышают выигрыш от параллельной сборки. По этой же причине в режиме Server GC зафиксировано больше сборок мусора (27–33 в Gen0 против стабильных 24 у Workstation, а также срабатывания Gen1 и Gen2). При добавлении фоновой нагрузки в виде 4K видео время работы заметно выросло в обоих режимах, но сильнее просел Server GC (до 105,47 мс), так как его фоновые потоки сборки начали конкурировать за процессорное время с декодированием видео.
>>>>>>> 51de407 (feat(gc): compare workstation and server gc modes)
