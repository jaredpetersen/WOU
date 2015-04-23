module Main where
import System.Environment

{-      Lab 4 Jared Petersen    -}

-- Example
square :: Integer -> Integer
square x = x^2
{- ANSWER: The square of 93240823948293048 is 8693851250556578380656712885130304 -}

-- Problem #1
gallons :: Double -> Double
gallons x = x * 0.264172

usd :: Double -> Double
usd x = x * 0.79

price :: Double -> Double -> Double
price x y = usd y / gallons x
{- ANSWER: The price of fuel in USD for 62.3 liters of fuel at 78.4 Canadian dollars is ~ $3.76 -}

-- Problem #2
radian :: Double -> Double
radian x = x * 0.0174532925

flightDistance :: (Double, Double) -> (Double, Double) -> Double
flightDistance (w,x) (y,z) = a * acos(cos(degW) * cos(degY) * cos(degX - degY) + sin(degW) * sin(degY))
        where degW = radian w
              degX = radian x
              degY = radian y
              degZ = radian z
              a = 3693
{- ANSWER: This dstance between the point at 45°N,122°W to the point at 21°N,158°W is 6825.203589344222 nmi (nautical miles) -}

-- Problem #3
factorial :: Integer -> Integer
factorial x
        | x < 0 = error "Cannot Use Negative Integers"
        | otherwise = (foldl (*) 1 [1..x])
{- ANSWER: The factorial of 99 is 933262154439441526816992388562667004907159682643816214685929638952175999932299156089414639761565182862536979208272237582511852109168640000000000000000000000 -}

-- Problem #4
isEven :: Integer -> String
isEven x =
        if (x `mod` 2 == 0)
                then "Even"
                else "Odd"
{- ANSWER: The even/odd status of 5 is odd. The even/odd status of 2 is even. -}

-- Problem #5
sumOfCubes = sum([x^2 | x <- [1000..2000], x `mod` 2 /= 0])
{- ANSWER: The sum of cubes of all the odd numbers between 1000 and 2000 is 1166666500 -}

-- Problem #6
closedFormSum :: Double -> Double
closedFormSum x = (x * (x + 1) * (2 * x + 1)) / 6
{- ANSWER: The closed form solution for an input of 1000 is 3.338335e8 -}

-- Problem #7
countList :: Eq a => [a] -> a -> Integer
countList a b = sum([ 1 | as <- a, as == b])

countRecur :: (Eq a, Num b) => [a] -> a -> b
countRecur [] b = 0
countRecur (a:as) b
        | b == a = (countRecur as b) + 1
        | otherwise = countRecur as b
{- ANSWER: There are 7 w's in the string "western oregon wolves (wow) win winter wrestling" -}

-- Problem #8
maxList :: (Ord a) => [a] -> a
maxList [] = error "The List is Empty"
maxList [a] = a
maxList (a:as) = max a (maxList as)
{- ANSWER: The max character in the the string "Cowabunga dudez" is 'z' -}

-- Problem #9
removeSpace :: String -> String
removeSpace x = filter (/= ' ') x

removeEven :: [Integer] -> [Integer]
removeEven x = filter (not . even) x

doubleAll :: [Integer] -> [Integer]
doubleAll x = map (^2) x

contains55 :: [Integer] -> Bool
contains55 x = any (55==) x

allOdd :: [Integer] -> Bool
allOdd x = all (odd) x
{- ANSWER: The string "How are you this evening?" is "Howareyouthisevening?" when the spaces are removed. 
   ANSWER: The list [0,2,8,0,1] becomes [1] when the even numbers are filtered out. 
   ANSWER: The list [0,1,2,3,4] become [0,1,4,9,16] when all of the numbers are doubled. 
   ANSWER: The list [0,55,9,42] contains 55.
   ANSWER: All items in the list [1,3,5,9] are odd. -}

-- Problem #10
isPrime :: Integer -> Bool
isPrime x
        | x == 0 = False
        | x == 1 = False
        | x == 2 = True
        | (countList (map (x `mod`) [2..x]) 0) == 1 = True
        | otherwise = False
-- listPrime 1000 1020 0 []
-- x is lowest nth prime, y is highest nth prime, c is count (used for recursion), z is list (used for recursion)
listPrime :: Int -> Int -> Integer -> [Integer] -> [Integer]
listPrime x y c z
        | length z == y = reverse (take ((y-x) + 1) (reverse z))
        | (isPrime c) == True = listPrime x y (c + 1) (z ++ [c])
        | (isPrime c) == False = listPrime x y (c + 1) z
{- ANSWER: The 1000th through the 1020th prime numbers are [7919,7927,7933,7937,7949,7951,7963,7993,8009,8011,8017,8039,8053,8059,8069,8081,8087,8089,8093,8101,8111] -}

-- Problem #11
factor :: Integer -> [Integer]
factor x
        | x < 2 = []
        | x == 2 = [2]
        | otherwise = filter (isPrime) [y | y <- [1..x], (x `mod` y) == 0]
{- ANSWER: The prime factor of 175561 is [419]. The prime factors of 62451532000 are [2,5,11,13,23,47,101] -}
-- Little bit of a side not on the input of 62451532000: GHCI returns the correct list minus the last bracket and then sits. Not sure what should be done to fix this.

-- Problem #12
main :: IO ()
main = do
args <- getArgs
print $ factor (read (head args)::Integer)
{- ANSWER: The prime factors of 5698 are [2,7,11,37] -}
