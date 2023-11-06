package bot

import (
	"net/http"
	"fmt"
	"io"
	"strings"
	"bloodreddawn.com/IsetGo/config"
)


func getWebVersion(url string) (string, error) {
    resp, err := http.Get(url)
    if err != nil {
        return "", fmt.Errorf("GET error: %v", err)
    }
    defer resp.Body.Close()

    if resp.StatusCode != http.StatusOK {
        return "", fmt.Errorf("Status error: %v", resp.StatusCode)
    }

    data, err := io.ReadAll(resp.Body)
    if err != nil {
        return "", fmt.Errorf("Read body: %v", err)
    }

    return string(data), nil
}

func updateAvail() bool {
	webVers, _ := getWebVersion(config.BotUpdateUrl)
	webParts := strings.Split(webVers, ".")

	thisVers := config.BotVersion
	thisParts := strings.Split(thisVers, ".")
	
	for index, line := range webParts {
		if (line != thisParts[index]) {
			return true
		}
    }
	return false
}

func CheckUpdate() string {
	if (updateAvail()) {
		return "\n An Update is available and can be downloaded from " + config.GithubLatestUrl
	}
	return "\n This is the most current release - no need to update!"
}