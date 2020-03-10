#!/bin/bash

# Set script parameters
SPOTIFY_PATH="$HOME"/.cache/spotify/Users/
COLLECTIONS="$(find "$SPOTIFY_PATH" -name 'collection*')"
echo "Searching in for collection caches in: "$SPOTIFY_PATH""

# Check for collection files
if [[ -z "$COLLECTIONS" ]]; then
	echo "No collection caches found. Exiting"
	exit 1
else
	echo "Found collection caches"
fi

echo "Deleting:"
for C in "$COLLECTIONS"; do
	echo "$C"
	rm -f "$C"
done

echo "Restarting Spotify"
pkill spotify
nohup spotify > /dev/null 2>&1 < /dev/null & 
