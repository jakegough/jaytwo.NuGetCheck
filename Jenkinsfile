library 'JenkinsBuilderLibrary'

helper.gitHubUsername = 'jakegough'
helper.gitHubRepository = 'jaytwo.NuGetCheck'
helper.gitHubTokenCredentialsId = 'github-personal-access-token-jakegough'
helper.nuGetCredentialsId = 'nuget-org-jaytwo'
helper.dockerImageName = 'jaytwo.nugetcheck'
helper.dockerRegistry = null // null for docker hub
helper.dockerRegistryCredentialsId = 'userpass-dockerhub-jakegough'
helper.xunitTestResultsPattern = 'out/testResults/**/*.trx'

helper.run('linux && make && docker', {
    def timestamp = helper.getTimestamp()
    def safeJobName = env.JOB_NAME.replaceAll('[^A-Za-z0-9]', '_').toLowerCase()
    def dockerLocalTag = "jenkins__${safeJobName}__${timestamp}"
    
    withEnv(["DOCKER_TAG=${dockerLocalTag}", "TIMESTAMP=${timestamp}"]) {
        try {
            stage ('Build') {
                sh "make docker-builder"
                sh "make docker"
            }
            stage ('Unit Test') {
                sh "make docker-unit-test-only"
            }
            stage ('Pack') {
                if(env.BRANCH_NAME == 'master'){
                    sh "make docker-pack-only"
                } else {
                    sh "make docker-pack-beta-only"
                }
            }
            if(env.BRANCH_NAME == 'master' || env.BRANCH_NAME == 'develop'){
                stage ('Publish NuGet') {
                    helper.pushNugetPackage('out/packed')
                }
                
                stage ('Publish Docker') {                        
                    helper.dockerLogin()
                    
                    def dockerRegistryImage = helper.getDockerRegistryImageName()
                    
                    if(env.BRANCH_NAME == 'master'){
                        def dockerRegistryImageRelease = "${dockerRegistryImage}:${timestamp}"
                        
                        helper.tagDockerImage(dockerLocalTag, dockerRegistryImage)
                        helper.tagDockerImage(dockerLocalTag, dockerRegistryImageRelease)
                        
                        helper.pushDockerImage(dockerRegistryImage)
                        helper.pushDockerImage(dockerRegistryImageRelease)
                        
                        helper.removeDockerImage(dockerRegistryImage)
                        helper.removeDockerImage(dockerRegistryImageRelease)
                        
                    } else {
                        def dockerRegistryImageBeta = "${dockerRegistryImage}:beta"
                        def dockerRegistryImagePrerelease = "${dockerRegistryImageBeta}-${timestamp}"
                        
                        helper.tagDockerImage(dockerLocalTag, dockerRegistryImageBeta)
                        helper.tagDockerImage(dockerLocalTag, dockerRegistryImagePrerelease)
                        
                        helper.pushDockerImage(dockerRegistryImageBeta)
                        helper.pushDockerImage(dockerRegistryImagePrerelease)
                        
                        helper.removeDockerImage(dockerRegistryImageBeta)
                        helper.removeDockerImage(dockerRegistryImagePrerelease)
                    }
                }
            }
        }
        finally {
            // inside the withEnv()
            sh "make docker-clean"
        }
    }
})
