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

func CheckUpdate() {
	webVers, _ := getWebVersion(config.BotUpdateUrl)
	thisVers := config.BotVersion
	_ = thisVers
	webParts := strings.Split(webVers, ".")

	for index, line := range webParts {
        fmt.Printf("Line %d: %s\n", index, line)
    }
}