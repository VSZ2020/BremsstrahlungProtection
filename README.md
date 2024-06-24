# Bremsstrahlung Protection (BSP)

Program package for calculation of protection from bremsstrahlung of extended beta-radiation sources.

Certificate of state registration of the computer program [№2021610532](https://www.fips.ru/publication-web/publications/document?type=doc&tab=PrEVM&id=1C3B52C5-02FF-43D6-8298-BA362D1E6A74) dated 15 Jan 2021.

![image](https://github.com/VSZ2020/Bremsstrahlung_Protection/assets/62175152/ea82a2d1-ffc2-43d9-b5b0-6c568e4b8f87)

All input parameters to the calculation are grouped into 4 tabs:
- `Source` - radiation source parameters;
- `Shielding` - parameters of the outer layers of protection of the extended source;
- `Buildup` - settings of the method of calculation of buildup factors;
- `Output` - parameters of the output value including the dose type.

Available radionuclides for source inclusion:
- P-32,
- Kr-85,
- Sr-89,
- Sr-90,
- Y-90,
- Zr-95,
- Nb-95,
- Ru-103,
- Ru-106+Rh-106,
- Ce-144,
- Pr-144, 
- Pm-147,
- Tl-204.

Available materials:
- Aluminum,
- Concrete,
- Air,
- Water,
- Tungsten,
- Iron,
- Copper,
- Lead,
- Carbon,
- Uranus,
- SrTiO3 (strontium titanate).

## Tab "Source"
"BSP" performs calculations for several extended source geometries: cylinder and rectangular parallelepiped. The case of a point source is also present. As the package develops, new shapes will be added. In the left part of the window the set of radionuclides included in the extended source and their activity is set. In the center - characteristics of bremsstrahlung (average group energy for quanta of bremsstrahlung, energy flux of bremsstrahlung). On the right - source shape and its geometrical characteristics, material of the extended source, its density, effective atomic number. It is also possible to specify the cutoff energy below which the calculation will not be performed.


## Tab "Shielding"
Here you specify a set of shielding layers that attenuate the bremsstrahlung on the way to the radiation registration point. The material, thickness and density are specified for the shielding layer. By default, the density of the selected material is substituted.

![image](https://github.com/VSZ2020/Bremsstrahlung_Protection/assets/62175152/c37d5762-0839-4cd4-b13c-fdca16f81677)

## Tab "Buildup"
Contains methods of calculation of buildup factors for homogeneous and heterogeneous medium. The buildup factor can be left out of the calculation. For this purpose it is enough to uncheck the box "Include scattered radiation".

![image](https://github.com/VSZ2020/Bremsstrahlung_Protection/assets/62175152/b39178a2-a7d0-4f0b-85ba-f639fd6de4fa)

The following methods of calculating accumulation factors are available.
For a homogeneous medium:
- Taylor's two-exponential approximation;
- Geometric progression.
For heterogeneous environments:
- Broder;
- Last layer;
- Weighted accumulation factor.

## Tab "Output"
Here you specify the type of output dosimetric value. Available options:
- Effective dose;
- Equivalent dose;
- Air kerma;
- Exposure dose;
- Ambient dose equivalent;
- Personal dose equivalent Hp(10);
- Personal dose equivalent Hp(0.07).

![image](https://github.com/VSZ2020/Bremsstrahlung_Protection/assets/62175152/0ad1054b-0124-4f71-b811-a8b582ead28c)

Depending on the selected dosimetric value, additional filters are set: exposure geometry and exposed organ/tissue.
On the same tab the distance to the registration point in centimeters and a number of additional parameters are specified.

---

Програмный пакет для расчета защиты от тормозного излучения протяженных источников бета-излучения.

Свидетельство о государственной регистрации программы для ЭВМ [№2021610532](https://www.fips.ru/publication-web/publications/document?type=doc&tab=PrEVM&id=1C3B52C5-02FF-43D6-8298-BA362D1E6A74) от 15 января 2021 г.

![image](https://github.com/VSZ2020/Bremsstrahlung_Protection/assets/62175152/ea82a2d1-ffc2-43d9-b5b0-6c568e4b8f87)

Все входные параметры к расчету сгруппированы по 4 вкладкам:
- `Source` - параметры источника излучения;
- `Shielding` - параметры внешних слоев защиты протяженного источника;
- `Buildup` - настройки метода расчета факторов накопления;
- `Output` - параметры выходной величины.

Доступные радионуклиды для включения в состав источника: 
- P-32,
- Kr-85,
- Sr-89,
- Sr-90,
- Y-90,
- Zr-95,
- Nb-95,
- Ru-103,
- Ru-106+Rh-106,
- Ce-144,
- Pr-144, 
- Pm-147,
- Tl-204.

Доступные материалы:
- Алюминий,
- Бетон,
- Воздух,
- Вода,
- Вольфрам,
- Железо,
- Медь,
- Свинец,
- Углерод,
- Уран,
- SrTiO3 (титанат стронция).

## Вкладка "Source"
"BSP" выполняет расчеты для нескольких геометрий протяженных источников: цилиндр и прямоугольный параллелепипед. Также присутствует случай точечного источника. По мере развития пакета будут добавляться новые формы. В левой части окна задается набор радионуклидов, входящих в состав протяженного источника, и их активность. В центре - характеристики тормозного излучения (средняя энергия группы для квантов тормозного излучения, поток энергии тормозного излучения). Справа - форма источника и её геометрические характеристики, материал протяженного источника, его плотность, эффективный атомный номер. Также можно указать энергию отсечки, ниже которой расчет проводиться не будет.

## Вкладка "Shielding"
Здесь задается набор слоев защиты, ослабляющих тормозное излучение на пути к точке регистрации излучения. Для слоя защиты указывается материал, толщина и плотность. По-умолчанию подставляется плотность выбранного материала.
  
![image](https://github.com/VSZ2020/Bremsstrahlung_Protection/assets/62175152/c37d5762-0839-4cd4-b13c-fdca16f81677)

## Вкладка "Buildup"
Содержит методы расчета факторов накопления для гомогенной и гетерогенной среды. Фактор накопления можно не учитывать при расчете. Для этого достаточно снять галочку с поля "Include scattered radiation".

![image](https://github.com/VSZ2020/Bremsstrahlung_Protection/assets/62175152/b39178a2-a7d0-4f0b-85ba-f639fd6de4fa)

Доступны следующие методы расчета факторов накопления.
Для гомогенной среды:
- Тейлора в двухэкспоненциальном приближении;
- Геометрической прогрессии.
Для гетерогенной среды:
- Бродера;
- Последнего слоя;
- Взвешенного фактора накопления.

## Вкладка "Output"
Здесь задается тип выходной дозиметрической величины. Доступные варианты:
- Эффективная доза;
- Эквивалентная доза;
- Воздушная керма;
- Экспозиционная доза;
- Амбиентный эквивалент дозы;
- Индивидуальный эквивалент дозы Hp(10);
- Индивидуальный эквивалент дозы Hp(0.07).

![image](https://github.com/VSZ2020/Bremsstrahlung_Protection/assets/62175152/0ad1054b-0124-4f71-b811-a8b582ead28c)

В зависимости от выбранной дозиметрической величины задаются дополнительные фильтры: геометрия облучения и облучаемый орган/ткань.
На этой же вкладке указывается расстояние до точки регистрации в сантиметрах и ряд дополнительных параметров.
