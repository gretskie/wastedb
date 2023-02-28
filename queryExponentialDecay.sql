SELECT SUM(items.activityOnRef*EXP(-(LOG(2)/isotopes.halflifeHours)*DATEDIFF(hour,items.referenceDateTime,GETDATE()))) AS totalActivity, isotopes.symbol, 
wasteCatagory.limitQuant, 
(SUM(items.activityOnRef*EXP(-(LOG(2)/isotopes.halflifeHours)*DATEDIFF(hour,items.referenceDateTime,GETDATE())))/wasteCatagory.limitQuant) AS limitPercentage
FROM items 
INNER JOIN isotopes ON isotopes.id = items.isotope 
LEFT JOIN wasteCatagory ON wasteCatagory.id = isotopes.wasteCatagory 
WHERE items.disposed = 0
GROUP BY isotopes.symbol, wasteCatagory.limitQuant
