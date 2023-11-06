package bot

import (
	"net/http"
	"fmt"
	"io"
	"os"
	"strings"
	"strconv"
	"bloodreddawn.com/IsetGo/config"
)

var (
	webVers string
	webVersion []int
	localVers string
	localVersion []int
)

func init() {
	getWebVersion()
	localVers = config.BotVersion
}

func getWebVersion() (string, error) {
	ret := config.BotVersion
    resp, err := http.Get(config.BotUpdateUrl)
    if err != nil {
        return ret, fmt.Errorf("GET error: %v", err)
    }
    defer resp.Body.Close()

    if resp.StatusCode != http.StatusOK {
        return ret, fmt.Errorf("Status error: %v", resp.StatusCode)
    }

    data, err := io.ReadAll(resp.Body)
    if err != nil {
        return ret, fmt.Errorf("Read body: %v", err)
    }
	ret = string(data)
	webVers = ret
	for _, line := range strings.Split(ret, ".") {
		tmp, err := strconv.Atoi(line)
		if err != nil {
			tmp = 2
		}
		webVersion = append(webVersion, tmp)
    }
	
    return ret, nil
}

func updateAvail() bool {
	fmt.Println("Latest Version: " + webVers)
	fmt.Println("Installed Version: " + localVers)
	
	//loop through values in thisParts array and try to convert the values to int
	for _, line := range strings.Split(localVers, ".") {
		tmp, err := strconv.Atoi(line)
		if err != nil {
			tmp = 2
		}
		localVersion = append(localVersion, tmp)
    }
	
	for i := 0; i < len(localVersion); i++ { 
        if (webVersion[i] > localVersion[i]) {
			return true
		}
    } 
	
	return false
}

func CheckUpdate(autodownload bool) string {
	if (updateAvail()) {
		if (!autodownload) {
			return "\nAn Update is available and can be downloaded from " + config.GithubLatestUrl
		} else {
			path, err := os.Getwd()
			if err != nil {
				return "\nAn update is available but was unable to automatically download! Please manually update!"
			}
			dlUrl := config.UpdateDLUrl + webVers + ".zip"
			fmt.Println(dlUrl)
			downloadFile(path + "\\update.zip", dlUrl)
			return "\nAn update is available and has been downloaded to the server!"
		}
	}
	return "\nThis is the most current release - no need to update!"
}

func downloadFile(filepath string, url string) (err error) {

  // Create the file
  out, err := os.Create(filepath)
  if err != nil  {
    return err
  }
  defer out.Close()

  // Get the data
  resp, err := http.Get(url)
  if err != nil {
    return err
  }
  defer resp.Body.Close()

  // Check server response
  if resp.StatusCode != http.StatusOK {
    return fmt.Errorf("bad status: %s", resp.Status)
  }

  // Writer the body to file
  _, err = io.Copy(out, resp.Body)
  if err != nil  {
    return err
  }

  return nil
}