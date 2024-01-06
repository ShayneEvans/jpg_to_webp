﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using SixLabors.ImageSharp.Formats.Webp;

const int QUALITY = 100;
const int LOSSLESSQUALITY = 100;

//random bits put onto the end of the file name
ulong getRandomBits() {
    byte[] bytes = new byte[32];

    using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
        rng.GetBytes(bytes);
    }

    ulong randomInt = BitConverter.ToUInt64(bytes, 0);
    return randomInt;
}

//Method used to convert from jpg to webp
void convertImages(string basePath, string outputPath, string directoryName) {
    string imagePath = Path.Combine(basePath, directoryName);
    string[] images = Directory.GetFiles(imagePath);
    string outputFolder = Path.Combine(outputPath, directoryName);

    //Create folder if it doesn't exist
    if (!Directory.Exists(Path.Combine(outputPath, $"{directoryName}"))) {
        Directory.CreateDirectory(Path.Combine(outputPath, $"{directoryName}"));
    }

    foreach (string imageName in images) {
        //Constructing new image file name
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
        ulong randomBits = getRandomBits();
        string newFileName = $"{fileNameWithoutExtension}-{randomBits}.webp";
        string outputDir = Path.Combine(outputFolder, newFileName);

        try {
            using (Image img = Image.Load(imageName)) {
                img.Mutate(x => x.Resize(640, 360));
                img.Save(outputDir, new WebpEncoder { Quality = QUALITY, NearLosslessQuality = LOSSLESSQUALITY });
            }
        }

        catch (UnauthorizedAccessException e) {
            Console.WriteLine($"Access denied: {e.Message}");
        }
    }
}

//Takes a starting and ending directory inside of a base path (start and end folders are numbers e.g, 130-135) and outputs the conversion folders to outputPath directory.
void batchConvertImages(string basePath, string outputPath, int startDirectory, int endDirectory) {
    for(int i = startDirectory; i <= endDirectory; i++) {
        convertImages(basePath, outputPath, i.ToString());
    }
}

//Set base path, and output path here
string basePath = "PATH_TO_BASE_FOLDER";
string outputPath = "PATH_TO_OUTPUT_FOLDER";

//Starting directory and ending directory are a range of numbers, script will go through the start to the end and conver all files inside to webp.
int startDir = 130;
int endDir = 131;

batchConvertImages(basePath, outputPath, startDir, endDir);