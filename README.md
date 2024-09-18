<div align="center">
<h1>Resonance Extractor</h1>
An Improved app to extract Audio files from Genshin Impact or any other unreal engine .pck files.

<h3></h3>

<div>
 <image src="https://github.com/CosmicPredator/Resonance-Extractor/tree/master/Docs/image.jpg" width="200"/>
</div>

</div>

# 

Highly Inspired from [@dvingerh](https://github.com/dvingerh)'s [genshin-audio-exporter](https://github.com/dvingerh/genshin-audio-exporter).

# Usage
- You can get any .pck files to extract from at `Genshin Impact game\GenshinImpact_Data\StreamingAssets\AudioAssets`.
- Add any .pck file to the app using "Select File" button.
- Specify the output folder where the extracted audio should reside by clicking "Select Folder" Button.
- Choose the audio formats you want. Available formats are,
	- WAV
	- FLAC
	- MP3
	- OGG
- Click on "Start" button to start the extraction.
- Enjoy the music 🙂‍↔️

# Special Thanks
This project heavily relies on these open source projects,
- [genshin-audio-exporter](https://github.com/dvingerh/genshin-audio-exporter)
- [QuickBMS](https://aluigi.altervista.org/quickbms.htm)
- [VGMStreamCLI](https://vgmstream.org/)
- [FFMpeg](https://www.ffmpeg.org/)

# Additional Notes
- A custom script is used to extract contents from `.pck` files. You can look at the script at `Libs\wavescan.bms`.
- A custom build of FFMpeg with only FLAC, MP3 and OGG capabilities is used to reduce disk footprint. (Binary Size: ~3.8mb)