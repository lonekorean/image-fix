# image-fix

Simple .NET utility to scale and crop an image to create a preview/thumbnail.

### Usage

This project provides a single class, `CodersBlock.ImageFix.ImageFixer`, which provides a single static function, `ResizeImage()`. This function takes the following parameters:

- `byte[] imageFile` - The image as a byte array
- `int scaleWidth` - Width to scale the image to
- `int maxheight` - Maximum height of the scaled image, if exceeded then cropping will occur
- `bool allowStretching` - If true, then the scaled width is allowed to exceed the original width
- `long quality` - The quality of the resulting JPG (100 is the max)

`ResizeImage()` returns a byte array for the new image.

Feel free to incorporate the source code, or just drop in the DLL directly (found in `/Built`).