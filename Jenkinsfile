library 'JenkinsBuilderLibrary@xunit'

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
    def dockerLocalTag = "jenkins__${helper.dockerImageName}__${timestamp}"
    
    try {
        withEnv(["DOCKER_TAG=${dockerLocalTag}", "TIMESTAMP=${timestamp}"]) {
            stage ('Build') {
                sh "make docker-build"
            }
            stage ('Unit Test') {
                sh "make docker-unit-test"
            }
            stage ('Integration Test') {
                sh "make docker-integration-test"
            }
            stage ('Pack') {
                if(env.BRANCH_NAME == 'master'){
                    sh "make docker-pack"
                } else {
                    sh "make docker-pack-beta"
                }
            }
            if(env.BRANCH_NAME == 'master' || env.BRANCH_NAME == 'develop'){
                stage ('Publish NuGet') {
                    helper.pushNugetPackage('out/packed')
                }
                
                stage ('Publish Docker') {                        
                    sh "make docker-image"
                    
                    helper.dockerLogin()
                    
                    def dockerRegistryImage = helper.getDockerRegistryImageName()
                    
                    if(env.BRANCH_NAME == 'master'){
                        helper.tagDockerImage(dockerLocalTag, dockerRegistryImage)
                        helper.pushDockerImage(dockerRegistryImage)
                        helper.removeDockerImage(dockerRegistryImage)
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
                    
                    helper.removeDockerImage(dockerLocalTag)
                }
            }
        }
    }
    finally {
        // not wrapped in a stage because it throws off stage history when cleanup happens because of a failed stage
        sh "make docker-cleanup"        
    }
})
